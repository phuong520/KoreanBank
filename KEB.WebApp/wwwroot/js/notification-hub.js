const userId = window.appUserId; 
console.log("User ID from Razor:", userId);

if (!userId) {
    console.warn("No user ID found, redirecting to login");
    // window.location.href = "/Commonweb/Login";
    //return;
}

// Khởi tạo kết nối SignalR mà không cần token trực tiếp
let connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7101/notify", {
        withCredentials: true // Gửi cookie tự động, bao gồm token HttpOnly
    })
    .build();

console.log("Starting SignalR connection...");

connection.start()
    .then(() => {
        console.log("SignalR connected");
        return connection.invoke("GetLatestNoti");
    })
    .catch(err => {
        console.error("SignalR connection error or invoke error:", err);
        if (err.statusCode === 401 || err.statusCode === 403) {
            window.location.href = "/Commonweb/Login";
        }
    });

connection.on("SendLatestNotifications", (notis) => {
    console.log("New notifications received:", notis);
    const unreadCount = notis.filter(n => !n.isRead).length;
    notis.forEach(noti => showNotification(noti, unreadCount));
});

connection.on("UpdateNotificationStatus", (notiId, isRead) => {
    console.log(`Notification ${notiId} updated, isRead: ${isRead}`);
    const page = document.body.dataset.page;

    if (page === "notification-index") {
        const row = document.querySelector(`.notification-item[data-id="${notiId}"]`);
        if (row) {
            if (isRead) {
                row.classList.remove("unread");
                row.querySelector(".is-read").textContent = "Đã đọc";
            } else {
                row.classList.add("unread");
                row.querySelector(".is-read").textContent = "Chưa đọc";
            }
        }
    } else {
        const item = document.querySelector(`#notification-list .notification-item[data-id="${notiId}"]`);
        if (item) {
            if (isRead) {
                item.classList.remove("unread");
            } else {
                item.classList.add("unread");
            }
            updateBadge();
        }
    }
});

function showNotification(noti, unreadCount) {
    const page = document.body.dataset.page;
    console.log("Showing notification:", noti, "Unread count:", unreadCount, "Page:", page);

    if (page === "notification-index") {
        const tableBody = document.querySelector(".notification-table tbody");
        if (!tableBody) {
            console.warn("Table body not found in notification index page");
            return;
        }

        const existingRow = tableBody.querySelector(`.notification-item[data-id="${noti.id}"]`);
        if (existingRow) {
            console.log(`Notification ${noti.id} already exists in table`);
            return;
        }

        const row = document.createElement("tr");
        row.classList.add("notification-item");
        if (!noti.isRead) {
            row.classList.add("unread");
        }
        row.dataset.id = noti.id;
        row.innerHTML = `
            <td>${noti.id}</td>
            <td>${noti.description}</td>
            <td class="is-read">${noti.isRead ? "Đã đọc" : "Chưa đọc"}</td>
            <td>${new Date(noti.createdTime).toLocaleString("vi-VN")}</td>
        `;
        row.addEventListener("click", function () {
            if (!noti.isRead) {
                markAsRead(noti.id, this);
            }
        });
        tableBody.prepend(row);
    } else {
        const notificationList = document.getElementById("notification-list");
        const badge = document.getElementById("notification-badge");

        if (!notificationList || !badge) {
            console.warn("Element notification-list or notification-badge not found");
            return;
        }

        const existingItem = notificationList.querySelector(`.notification-item[data-id="${noti.id}"]`);
        if (existingItem) {
            console.log(`Notification ${noti.id} already exists in dropdown`);
            return;
        }

        const li = document.createElement("li");
        li.classList.add("notification-item");
        if (!noti.isRead) {
            li.classList.add("unread");
        }
        li.dataset.id = noti.id;
        li.innerHTML = `
            <div>${noti.description}</div>
            <small>${new Date(noti.createdTime).toLocaleString("vi-VN")}</small>
        `;
        li.addEventListener("click", function () {
            if (!noti.isRead) {
                markAsRead(noti.id, this);
            }
        });

        const emptyMsg = notificationList.querySelector(".notification-empty");
        if (emptyMsg) {
            emptyMsg.remove();
        }

        notificationList.prepend(li);

        const items = notificationList.querySelectorAll(".notification-item");
        if (items.length > 7) {
            items[items.length - 1].remove();
        }

        badge.textContent = unreadCount;
    }
}

function markAsRead(notiId, element) {
    console.log("Đánh dấu thông báo đã đọc, ID:", notiId);
    fetch(`https://localhost:7101/api/Notification/markasread/${notiId}`, {
        method: "PUT",
        credentials: "include" // Gửi cookie tự động, bao gồm token HttpOnly
    })
        .then(response => {
            console.log("Mã trạng thái API markasread:", response.status);
            if (!response.ok) throw new Error(`HTTP ${response.status}`);
            return response.text();
        })
        .then(data => {
            console.log("Phản hồi API markasread:", data);
            if (data.includes("read")) {
                element.classList.remove("unread");
                if (element.querySelector(".is-read")) {
                    element.querySelector(".is-read").textContent = "Đã đọc";
                }
                updateBadge();
            } else {
                console.error("Lỗi khi đánh dấu đã đọc:", data);
            }
        })
        .catch(error => console.error("Lỗi khi gọi API markasread:", error));
}

function updateBadge() {
    const badge = document.getElementById("notification-badge");
    if (badge) {
        const unreadItems = document.querySelectorAll("#notification-list .notification-item.unread");
        badge.textContent = unreadItems.length;
        console.log("Cập nhật badge, số chưa đọc:", unreadItems.length);
    }
}