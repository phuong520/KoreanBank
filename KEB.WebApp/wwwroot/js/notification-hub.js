
// wwwroot/js/notification-hub.js
function getCookie(name) {
    const value = `; ${ document.cookie } `;
    const parts = value.split(`; ${ name }=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

let connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7101/notify", {
        accessTokenFactory: () => getCookie("token") || ""
    })
    .build();

console.log("JWT Token:", getCookie("token"));

connection.start()
    .then(() => {
        console.log("SignalR connected");
        return connection.invoke("GetLatestNoti");
    })
    .catch(err => console.error("SignalR connection error or invoke error:", err));

connection.on("SendLatestNotifications", (notis) => {
    console.log("New notifications received:", notis);
    notis.forEach(noti => showNotification(noti));
});

function showNotification(message) {
    const notificationList = document.getElementById("notification-list");
    const badge = document.getElementById("notification-badge");

    if (!notificationList || !badge) {
        console.warn("Element notification-list or notification-badge not found");
        return;
    }

    const li = document.createElement("li");
    li.classList.add("notification-item");
    if (!message.isRead) {
        li.classList.add("unread");
    }

    const createdTime = new Date(message.createdTime).toLocaleString();
    li.innerHTML = `
    < div class="notification-content" > ${ message.description }</div >
        <div class="notification-time">${createdTime}</div>
`;

    const emptyMsg = notificationList.querySelector(".notification-empty");
    if (emptyMsg) {
        emptyMsg.remove();
    }

    notificationList.prepend(li);

    let currentCount = parseInt(badge.innerText) || 0;
    badge.innerText = currentCount + 1;
}
```