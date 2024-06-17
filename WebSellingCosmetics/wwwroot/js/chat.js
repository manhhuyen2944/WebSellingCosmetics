var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    console.log("SignalR Connected!");
}).catch(function (err) {
    return console.error(err.toString());
});
connection.on("UserConnected", function (ConnectionId) {
    var user = document.getElementById('nameUser');
    //user.value = ConnectionId;
    //user.text = ConnectionId;
    // Xử lý sự kiện khi người dùng kết nối
    console.log("User connected with AccountId: " + ConnectionId);
});

connection.on("UserDisconnected", function (ConnectionId) {
    //var user = document.getElementById('nameUser');
    //if (user.value == ConnectionId) {
    //    user.remove;
    //}
    // Xử lý sự kiện khi người dùng ngắt kết nối
    console.log("User disconnected with AccountId: " + ConnectionId);
});

connection.on("ReceiveMessage", function (messageModel) {
    // Handle received message
    var user1 = document.getElementById('nameUser').value;

    if (user1 == messageModel.fromUser && user1 !== '') {
        // Tạo một phần tử div mới
        var newMessageDiv = document.createElement("div");
        newMessageDiv.className = "message user1";

        // Lấy giá trị từ input có id="avatar"
        var avatarValue = document.getElementById('avatar').value;

        // Tạo một phần tử div cho avatar
        var avatarDiv = document.createElement("div");
        avatarDiv.className = "user-avatar";

        // Tạo thẻ img cho avatar và đặt giá trị src từ biến avatarValue
        var avatarImage = document.createElement("img");
        avatarImage.src = avatarValue;

        // Chèn thẻ img vào phần tử div cho avatar
        avatarDiv.appendChild(avatarImage);

        // Tạo nội dung tin nhắn
        var messageContent = document.createElement("div");
        messageContent.className = "message-content";
        messageContent.innerText = messageModel.message;

        // Chèn phần tử div cho avatar vào trước nội dung tin nhắn
        newMessageDiv.appendChild(messageContent);
        newMessageDiv.appendChild(avatarDiv);
        // Chèn phần tử div mới vào phần tử chứa tin nhắn
        var chatMessagesContainer = document.getElementById("chat-messages");
        chatMessagesContainer.appendChild(newMessageDiv);

        // Cuộn xuống cuối cùng để hiển thị tin nhắn mới
        chatMessagesContainer.scrollTop = chatMessagesContainer.scrollHeight;

        /*console.log(messageModel.fromUser + " says: " + messageModel.message);*/

    }
    else {

        var newMessageDiv = document.createElement("div");
        newMessageDiv.className = "message user2";
        
        var avatarAdminValue = document.getElementById('avatarAdmin').value;
        if (messageModel.fromUser !== "Admin") {
            avatarAdminValue = document.getElementById('avatarUser').value;
        }

        var avatarDiv = document.createElement("div");
        avatarDiv.className = "user-avatar";

        var avatarImage = document.createElement("img");
        if (messageModel.fromUser !== "Admin") {
            avatarImage.src = '/contents/Images/User/' + avatarAdminValue;
        }
        else {
            avatarImage.src = avatarAdminValue;
        }
       

        avatarDiv.appendChild(avatarImage);

        var messageContent = document.createElement("div");
        messageContent.className = "message-content";
        messageContent.innerText = messageModel.message;

        newMessageDiv.appendChild(avatarDiv);
        newMessageDiv.appendChild(messageContent);

        var chatMessagesContainer = document.getElementById("chat-messages");
        chatMessagesContainer.appendChild(newMessageDiv);

        chatMessagesContainer.scrollTop = chatMessagesContainer.scrollHeight;

    }

});

// người gủi <=> người gửi  khác người gủi false
/// nếu là admin <=> admin  , người dùng to fale
// admin nhìu người dùng for ds người dùng
// ấn mở box chat 
// xỷ lý nhiều yc cl 
function sendMessage() {

    var user = document.getElementById('nameUser').value; // replace with actual user info
    var message = document.getElementById("user-input").value;
    var touser = "Admin";
    if (user !== '' && message.trim() !== '') {
        var messageObject = {
            fromUser: user,
            toUser: touser,
            message: message
        };

        $.ajax({
            url: '/Chat/SendMessage',
            type: 'POST',
            data: {
                fromUserId: user,
                toUserId: touser,
                content: message
            },
            success: function (data) {
                //console.log("Message sent successfully");
                //console.log(data);  // Log the response data if needed
            },
            error: function (xhr, status, error) {
                console.error('Ajax error:', status, error);
                console.log(xhr.responseText);
            }
        });
        connection.invoke("SendMessage", messageObject).catch(function (err) {
            return console.error(err.toString());
        });
        // Clear input fields
        
        document.getElementById('user-input').value = '';
    }
}
function sendMessageAdmin() {

    var user = "Admin"; // replace with actual user info
    var message = document.getElementById("user-input").value;
    var touser = document.getElementById("touser").value;
    if (user !== '' && message.trim() !== '') {
        var messageObject = {
            fromUser: user,
            toUser: touser,
            message: message
        };
        $.ajax({
            url: '/Chat/SendMessage',
            type: 'POST',
            data: {
                fromUserId: user,
                toUserId: touser,
                content: message
            },
            success: function (data) {
                //console.log("Message sent successfully");
                //console.log(data);  // Log the response data if needed
            },
            error: function (xhr, status, error) {
                console.error('Ajax error:', status, error);
                console.log(xhr.responseText);
            }
        });
        connection.invoke("SendMessage", messageObject).catch(function (err) {
            return console.error(err.toString());
        });
        // Clear input fields

        document.getElementById('user-input').value = '';
    }
}


