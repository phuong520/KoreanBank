// wwwroot/js/notification-hub.js
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}

// Tạo kết nối đến SignalR Hub
let connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7101/notify", {
        accessTokenFactory: () => getCookie("token") || ""  // trả token từ cookie
    })
    .build();
console.log("JWT Token:", getCookie("token"));

// Bắt đầu kết nối
connection.start()
    //.then(() => console.log("SignalR connected"))
//.catch(err => console.error("SignalR connection error:", err));
    .then(() => {
        console.log("SignalR connected");
        return connection.invoke("GetLatestNoti");
    })
    .catch(err => console.error("SignalR connection error or invoke error:", err));

// Lắng nghe sự kiện SendLatestNotifications với danh sách thông báo
connection.on("SendLatestNotifications", (notis) => {
    console.log("New notifications received:", notis);

    // Hiển thị từng thông báo
    notis.forEach(noti => showNotification(noti));
});

// Hàm hiển thị 1 thông báo lên UI
function showNotification(message) {
    const notificationList = document.getElementById("notification-list");
    const badge = document.getElementById("notification-badge");

    if (!notificationList || !badge) {
        console.warn("Element notification-list or notification-badge không tìm thấy");
        return;
    }

    const li = document.createElement("li");
    li.classList.add("notification-item");
    if (!message.isRead) {
        li.classList.add("unread");
    }

    const createdTime = new Date(message.createdTime).toLocaleString();

    li.innerHTML = `
        <div class="notification-content">${message.description}</div>
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

