// notification-hub.js
document.addEventListener("DOMContentLoaded", function () {
    // Tạo kết nối đến hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notifyHub") // Sử dụng đường dẫn đã cấu hình trong program.cs
        .withAutomaticReconnect() // Tự động kết nối lại khi mất kết nối
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // Đăng ký handler cho sự kiện SendLatestNotifications
    connection.on("SendLatestNotifications", function (notifications) {
        console.log("Nhận được thông báo mới:", notifications);

        // Hiển thị thông báo
        updateNotificationUI(notifications);
    });

    // Hàm cập nhật UI thông báo
    function updateNotificationUI(notifications) {
        // Lấy element hiển thị số lượng thông báo
        let notificationBadge = document.getElementById("notification-badge");
        if (notificationBadge) {
            notificationBadge.textContent = notifications.length;
            notificationBadge.style.display = notifications.length > 0 ? "inline-block" : "none";
        }

        // Lấy element hiển thị danh sách thông báo
        let notificationList = document.getElementById("notification-list");
        if (notificationList) {
            // Xóa tất cả thông báo cũ
            notificationList.innerHTML = "";

            // Thêm các thông báo mới
            if (notifications.length > 0) {
                notifications.forEach(function (noti) {
                    let item = document.createElement("li");
                    item.className = noti.isRead ? "notification-item read" : "notification-item unread";

                    // Tạo nội dung thông báo
                    let content = document.createElement("div");
                    content.className = "notification-content";
                    content.textContent = noti.content;

                    // Tạo thời gian thông báo
                    let time = document.createElement("div");
                    time.className = "notification-time";
                    time.textContent = new Date(noti.createdDate).toLocaleString();

                    // Thêm vào item
                    item.appendChild(content);
                    item.appendChild(time);

                    // Thêm vào danh sách
                    notificationList.appendChild(item);
                });
            } else {
                // Hiển thị thông báo khi không có thông báo nào
                let emptyItem = document.createElement("li");
                emptyItem.className = "notification-empty";
                emptyItem.textContent = "Không có thông báo mới";
                notificationList.appendChild(emptyItem);
            }
        }
    }

    // Khởi động kết nối
    async function startConnection() {
        try {
            await connection.start();
            console.log("Kết nối SignalR thành công!");
        } catch (err) {
            console.error("Lỗi kết nối SignalR:", err);
            // Thử kết nối lại sau 5 giây
            setTimeout(startConnection, 5000);
        }
    }

    // Xử lý khi kết nối bị đóng
    connection.onclose(async () => {
        console.log("Kết nối SignalR bị đóng. Đang kết nối lại...");
        await startConnection();
    });

    // Khởi động kết nối
    startConnection();
});