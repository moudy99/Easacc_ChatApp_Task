const currentUserId = window.chatData.currentUserId;
const partnerId = window.chatData.partnerId;
const chatId = window.chatData.chatId;
const chatBody = document.getElementById("chatBody");

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

function sendMessage() {
    const content = document.getElementById("messageInput").value.trim();
    if (content !== "") {

        //
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
        for (let pair of formData.entries()) {
            console.log(pair[0] + ': ' + pair[1]);
        }

        fetch('/Chat/SendMessage', {
            method: 'POST',
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => { throw new Error(text); });
                }

              

                const message = `<div class="partner-message">
                <div class="message-content">
                    <p>${content}</p>
                    <span class="message-time">${formattedTime}</span>
                </div>
            </div>`;
                chatBody.insertAdjacentHTML('beforeend', message);
                chatBody.scrollTop = chatBody.scrollHeight;
                document.getElementById("messageInput").value = "";
            })
            .catch(error => console.error('Error:', error));
    }
}


function addMessageToChat(message) {
    console.log(message);
        const chatBody = document.getElementById("chatBody");
    const isMyMessage = message.senderId === currentUserId;

    const messageDiv = document.createElement("div");
        messageDiv.className = isMyMessage ? "partner-message" : "my-message";

    const contentDiv = document.createElement("div");
    contentDiv.className = "message-content";

    const contentP = document.createElement("p");
    contentP.textContent = message.content;

    const timeSpan = document.createElement("span");
    timeSpan.className = "message-time";
    timeSpan.textContent = new Date(message.sentAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: false });

    contentDiv.appendChild(contentP);
    contentDiv.appendChild(timeSpan);

    const img = document.createElement("img");
    img.src = "/imgs/defualt-user-img.png"; 
    img.alt = isMyMessage ? "Your Avatar" : "Partner Avatar";
    img.className = "user-image";

    if (isMyMessage) {
        messageDiv.appendChild(contentDiv);
    messageDiv.appendChild(img);
        } else {
        messageDiv.appendChild(img);
    messageDiv.appendChild(contentDiv);
        }

    chatBody.appendChild(messageDiv);
    chatBody.scrollTop = chatBody.scrollHeight;

    if (!isMyMessage) {
        fetch(`/Chat/MarkMessagesAsSeen?chatId=${chatId}`, {
            method: 'POST'
        }).catch(error => console.error('Error marking messages as seen:', error));
        }
    }

    fetch(`/Chat/GetMessages?chatId=${chatId}`)
        .then(response => response.json())
        .then(messages => {
        messages.forEach(addMessageToChat);
        })
        .catch(error => console.error('Error loading messages:', error));


