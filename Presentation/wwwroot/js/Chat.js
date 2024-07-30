const currentUserId = window.chatData.currentUserId;
const partnerId = window.chatData.partnerId;
const chatId = window.chatData.chatId;
const chatBody = document.getElementById("chatBody");
const previewArea = document.getElementById("previewArea");



function scrollToBottom() {
    try {
        this.chatBody.nativeElement.scrollTop =
            this.chatBody.nativeElement.scrollHeight;
    } catch (err) {
        console.error('Error scrolling to bottom:', err);
    }
}
scrollToBottom();

console.log(currentUserId);
console.log(partnerId);
console.log(chatId);

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.start().then(() => {
    console.log("SignalR Connected");
}).catch(err => console.error(err));

connection.on("ReceiveMessage", (message) => {
    addMessageToChat(message);
});

document.getElementById("sendButton").addEventListener("click", sendMessage);
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        sendMessage();
    }
});

document.getElementById("messageInput").addEventListener("input", function () {
    const maxLength = 100;
    const currentLength = this.value.length;
    document.getElementById("charCounter").textContent = maxLength - currentLength;
});

document.getElementById("sendImage").addEventListener("click", function () {
    document.getElementById("imageInput").click();
});

document.getElementById("sendFile").addEventListener("click", function () {
    document.getElementById("fileInput").click();
});

document.getElementById("imageInput").addEventListener("change", function () {
    if (this.files && this.files[0]) {
        previewFile(this.files[0], "image");
    }
});

document.getElementById("fileInput").addEventListener("change", function () {
    if (this.files && this.files[0]) {
        previewFile(this.files[0], "file");
    }
});

function previewFile(file, type) {
    const previewDiv = document.createElement("div");
    previewDiv.className = "preview-div";

    if (type === "image") {
        const img = document.createElement("img");
        img.className = "preview-image";
        img.src = URL.createObjectURL(file);
        img.onload = () => URL.revokeObjectURL(img.src);
        previewDiv.appendChild(img);
    } else if (type === "file") {
        const fileName = document.createElement("span");
        fileName.className = "preview-file-name";
        fileName.textContent = file.name;
        previewDiv.appendChild(fileName);
    }

    const deleteButton = document.createElement("button");
    deleteButton.className = "delete-preview";
    deleteButton.innerHTML = "&times;";
    deleteButton.addEventListener("click", function () {
        previewArea.removeChild(previewDiv);
    });

    previewDiv.appendChild(deleteButton);
    previewArea.appendChild(previewDiv);
}

function sendMessage() {
    const content = document.getElementById("messageInput").value.trim();
    if (content !== "") {
        const currentTime = new Date();
        const hours = currentTime.getHours().toString().padStart(2, '0');
        const minutes = currentTime.getMinutes().toString().padStart(2, '0');
        const formattedTime = `${hours}:${minutes}`;

        const formData = new FormData();
        formData.append("ChatId", chatId);
        formData.append("SenderId", currentUserId);
        formData.append("RecipientId", partnerId);
        formData.append("Content", content);
        formData.append("SentAt", currentTime);
        if (imageInput.files.length > 0) {
            formData.append("img", imageInput.files[0]);
        }
        fetch('/Chat/SendMessage', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text); });
                }
                return response.json();
            })
                .then(data => {
                    console.log('Response Data:', data);
                    const message = ` <div class="partner-message">
            <div class="message-content">
                <div class="message-text ${data.img ? 'time-text-align' : ''}">
                    <p>${content}</p>
                    <span class="message-time">${formattedTime}</span>
                </div>
                ${data.img ? `<img class="message-img" src="http://localhost:35848/${data.img}" />` : ''}
            </div>
        </div>
    `;
                    chatBody.insertAdjacentHTML('beforeend', message);
                    chatBody.scrollTop = chatBody.scrollHeight;
                    document.getElementById("messageInput").value = "";
                    document.getElementById("imageInput").value = "";
                    previewArea.innerHTML = "";
                    document.getElementById("charCounter").textContent = 100;
                })
            .catch(error => console.error('Error:', error));
    }
}

function sendFile(file, type) {
    const formData = new FormData();
    formData.append("ChatId", chatId);
    formData.append("SenderId", currentUserId);
    formData.append("RecipientId", partnerId);
    formData.append("SentAt", new Date());

    if (type === "image") {
        formData.append("Image", file);
    } else if (type === "file") {
        formData.append("File", file);
    }

    fetch('/Chat/SendFile', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
            }

            console.log(`${type.charAt(0).toUpperCase() + type.slice(1)} sent successfully.`);
        })
        .catch(error => console.error(`Error sending ${type}:`, error));
}

function addMessageToChat(message) {
    console.log(message);
    const chatBody = document.getElementById("chatBody");
    const isMyMessage = message.senderId === currentUserId;

    const messageDiv = document.createElement("div");
    messageDiv.className = isMyMessage ? "my-message" : "partner-message";

    const contentDiv = document.createElement("div");
    contentDiv.className = "message-content";

    const contentP = document.createElement("p");
    contentP.textContent = message.content;

    const timeSpan = document.createElement("span");
    timeSpan.className = "message-time";
    timeSpan.textContent = new Date(message.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    contentDiv.appendChild(contentP);
    contentDiv.appendChild(timeSpan);

    messageDiv.appendChild(contentDiv);
    chatBody.appendChild(messageDiv);
    chatBody.scrollTop = chatBody.scrollHeight;
}


