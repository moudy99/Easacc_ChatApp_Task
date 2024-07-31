const currentUserId = window.chatData.currentUserId;
const partnerId = window.chatData.partnerId;
const chatId = window.chatData.chatId;
const chatBody = document.getElementById("chatBody");
const previewArea = document.getElementById("previewArea");

function scrollToBottom() {
    try {
        chatBody.scrollTop = chatBody.scrollHeight;
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
        document.getElementById("fileInput").value = ""; // Clear file input
    }
});

document.getElementById("fileInput").addEventListener("change", function () {
    if (this.files && this.files[0]) {
        previewFile(this.files[0], "file");
        document.getElementById("imageInput").value = ""; // Clear image input
    }
});

function previewFile(file, type) {
    previewArea.innerHTML = ""; // Clear previous preview

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
        document.getElementById("imageInput").value = "";
        document.getElementById("fileInput").value = "";
    });

    previewDiv.appendChild(deleteButton);
    previewArea.appendChild(previewDiv);
}
function sendMessage() {
    const content = document.getElementById("messageInput").value.trim();
    if (content !== "" || document.getElementById("imageInput").files.length > 0 || document.getElementById("fileInput").files.length > 0) {
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

        if (document.getElementById("imageInput").files.length > 0) {
            formData.append("img", document.getElementById("imageInput").files[0]);
        }
        if (document.getElementById("fileInput").files.length > 0) {
            formData.append("document", document.getElementById("fileInput").files[0]);
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
                const message = `
                <div class="partner-message">
                    <div class="message-content">
                        <div class="message-text ${data.img || data.document ? 'time-text-align' : ''}">
                            <p>${content}</p>
                            <span class="message-time">${formattedTime}</span>
                        </div>
                        ${data.img ? `<img class="message-img" src="http://localhost:35848/${data.img}" />` : ''}
                        ${data.document ? `
                            <a href="http://localhost:35848/${data.document}" target="_blank" class="document-link">
                                <i class="fas fa-file-download"></i> Download Document
                            </a>
                        ` : ''}
                    </div>
                </div>
            `;
                chatBody.insertAdjacentHTML('beforeend', message);
                chatBody.scrollTop = chatBody.scrollHeight;
                document.getElementById("messageInput").value = "";
                document.getElementById("imageInput").value = "";
                document.getElementById("fileInput").value = "";
                previewArea.innerHTML = "";
                document.getElementById("charCounter").textContent = 100;
            })
            .catch(error => console.error('Error:', error));
    }
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

    if (message.img) {
        const img = document.createElement("img");
        img.className = "message-img";
        img.src = `http://localhost:35848/${message.img}`;
        contentDiv.appendChild(img);
    }

    if (message.document) {
        const docLink = document.createElement("a");
        docLink.href = `http://localhost:35848/${message.document}`;
        docLink.className = "document-link";
        docLink.target = "_blank";

        const icon = document.createElement("i");
        icon.className = "fas fa-file-download";

        docLink.appendChild(icon);
        docLink.appendChild(document.createTextNode(" Download Document"));

        contentDiv.appendChild(docLink);
    }

    messageDiv.appendChild(contentDiv);
    chatBody.appendChild(messageDiv);
    chatBody.scrollTop = chatBody.scrollHeight;
}