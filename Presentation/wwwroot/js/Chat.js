const currentUserId = window.chatData.currentUserId;
const partnerId = window.chatData.partnerId;
const chatId = window.chatData.chatId;
const chatBody = document.getElementById("chatBody");
const previewArea = document.getElementById("previewArea");
const recordingTime = document.getElementById("recordingTime");

let mediaRecorder;
let audioChunks = [];
let isRecording = false;
let recordingTimer;
let recordingDuration = 0;
let audioPreview = null;

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

document.getElementById("sendVoice").addEventListener("click", toggleVoiceRecording);

document.getElementById("imageInput").addEventListener("change", function () {
    if (this.files && this.files[0]) {
        previewFile(this.files[0], "image");
        document.getElementById("fileInput").value = ""; 
    }
});

document.getElementById("fileInput").addEventListener("change", function () {
    if (this.files && this.files[0]) {
        previewFile(this.files[0], "file");
        document.getElementById("imageInput").value = ""; t
    }
});

function previewFile(file, type) {
    previewArea.innerHTML = ""; 

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
    if (content !== "" || document.getElementById("imageInput").files.length > 0 || document.getElementById("fileInput").files.length > 0 || audioPreview) {
        const currentTime = new Date();
        const hours = currentTime.getHours().toString().padStart(2, '0');
        const minutes = currentTime.getMinutes().toString().padStart(2, '0');
        const formattedTime = `${hours}:${minutes}`;

        const formData = new FormData();
        formData.append("ChatId", chatId);
        formData.append("SenderId", currentUserId);
        formData.append("RecipientId", partnerId);
        formData.append("Content", content || "VOICE");
        formData.append("SentAt", currentTime);

        if (document.getElementById("imageInput").files.length > 0) {
            formData.append("img", document.getElementById("imageInput").files[0]);
        }
        if (document.getElementById("fileInput").files.length > 0) {
            formData.append("document", document.getElementById("fileInput").files[0]);
        }
        if (audioPreview) {
            formData.append("voice", audioPreview, "voice_message.wav");
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
                        <div class="message-text ${data.img || data.document || data.voice ? 'time-text-align' : ''}">
                            <p>${content || "Voice message"}</p>
                            <span class="message-time">${formattedTime}</span>
                        </div>
                        ${data.img ? `<img class="message-img" src="http://localhost:35848/${data.img}" />` : ''}
                        ${data.document ? `
                            <a href="http://localhost:35848/${data.document}" target="_blank" class="document-link">
                                <i class="fas fa-file-download"></i> Download Document
                            </a>
                        ` : ''}
                        ${data.voice ? `<audio controls src="http://localhost:35848/${data.voice}"></audio>` : ''}
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
                audioPreview = null;
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

    if (message.voice) {
        const audio = document.createElement("audio");
        audio.controls = true;
        audio.src = `http://localhost:35848/${message.voice}`;
        contentDiv.appendChild(audio);
    }

    messageDiv.appendChild(contentDiv);
    chatBody.appendChild(messageDiv);
    chatBody.scrollTop = chatBody.scrollHeight;
}

function toggleVoiceRecording() {
    const micButton = document.getElementById("sendVoice");

    if (!isRecording) {
        startRecording();
        micButton.innerHTML = '<i class="fas fa-stop" style="color: red;"></i>';
        micButton.classList.add("recording");
        recordingTime.style.display = "inline";
    } else {
        stopRecording();
        micButton.innerHTML = '<i class="fas fa-microphone"></i>';
        micButton.classList.remove("recording");
        recordingTime.style.display = "none";
    }
}

function startRecording() {
    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(stream => {
            mediaRecorder = new MediaRecorder(stream);
            mediaRecorder.start();
            isRecording = true;

            audioChunks = [];
            recordingDuration = 0;

            mediaRecorder.addEventListener("dataavailable", event => {
                audioChunks.push(event.data);
            });

            mediaRecorder.addEventListener("stop", createAudioPreview);

            recordingTimer = setInterval(() => {
                recordingDuration++;
                updateRecordingTime();
                if (recordingDuration >= 60) {
                    stopRecording();
                }
            }, 1000);
        });
}

function stopRecording() {
    if (mediaRecorder && isRecording) {
        mediaRecorder.stop();
        isRecording = false;
        clearInterval(recordingTimer);
    }
}

function updateRecordingTime() {
    const minutes = Math.floor(recordingDuration / 60);
    const seconds = recordingDuration % 60;
    recordingTime.textContent = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
}

function createAudioPreview() {
    const audioBlob = new Blob(audioChunks, { type: 'audio/wav' });
    audioPreview = audioBlob;

    const audioURL = URL.createObjectURL(audioBlob);

    previewArea.innerHTML = "";
    const previewDiv = document.createElement("div");
    previewDiv.className = "preview-div";

    const audio = document.createElement("audio");
    audio.controls = true;
    audio.src = audioURL;

    const deleteButton = document.createElement("button");
    deleteButton.className = "delete-preview";
    deleteButton.innerHTML = "&times;";
    deleteButton.addEventListener("click", function () {
        previewArea.removeChild(previewDiv);
        audioPreview = null;
    });

    previewDiv.appendChild(audio);
    previewDiv.appendChild(deleteButton);
    previewArea.appendChild(previewDiv);
}