var UserName = "USERNAME";
var CHATID = 0;
var sessionId = "";
var connection = new signalR
    .HubConnectionBuilder()
    .withUrl("/Hub/Default")
    .build();

connection.on("Connected", ConnectedToHub);

$(document).ready(function () {
    if (Notification.permission !== "granted") {
        Notification.requestPermission();
    }
});
async function StartConnection() {
    try {
        await connection.start();
    } catch {
        Swal.fire({
            title: 'اتصال قطع شد',
            text: 'اتصال مجدد در 5 ثانیه',
            timer: 5000,
            confirmButtonText: 'اتصال',
            showConfirmButton: true,
            timerProgressBar: true,
            didOpen: () => {
                Swal.showLoading()
                setTimeout(StartConnection, 5000);
            },
            willClose: () => {
            }
        }).then((result) => {
            if (result.isConfirmed) {
                StartConnection();
            }
        });
    }
}
connection.onclose(StartConnection);
StartConnection();
function ConnectedToHub(username, id) {
    UserName = username;
    console.log(`Hello ${username}`);
    sessionId = id;
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });

    Toast.fire({
        icon: 'success',
        title: `${username} متصل شدید`
    });
}

connection.on("UpdateChat", RecieveMessage);
function RecieveMessage(chatId) {
    if (chatId == CHATID) {
        LoadChat(chatId);
    }
}

$('#btnSendMessage').on('click', function () {
    sendMessage();
})
function sendMessage() {
    var msg = $("#txtMessage").val();
    if (msg) {
        connection.invoke('SendMessage', msg, $('#ChatOption').attr('data-chatId'));
        $("#txtMessage").val('');
        $("#txtMessage").focus();
    }
}
function SearchChat(event) {
    event.preventDefault();
    var text = event.target[0].value;
    if (text) {
        connection.invoke("SearchChat", text);
    }
    else {
        connection.invoke("LoadChats");
    }
}
connection.on("SearchResult", function (result) {
    if (result.isSuccess) {
        $('#chatList').html('');
        for (i in result.value) {
            var imgAddress = 'img/Default.jpg';
            if (result.value[i].imageSrc) {
                imgAddress = '/' + result.value[i].imageSrc;
            }
            $('#chatList').append(`<li data-chatid="${result.value[i].id}" onclick="LoadChat('${result.value[i].id}')">
                            <label>${result.value[i].title}</label>
                                <img src="${imgAddress}" />
                            <span> ${result.value[i].lastMessageDate} </span>
                        </li>`);
        }
    }
    else {
        console.log(result);
        alert(result.error);
    }
});

$('#txtMessage').keyup(function (e) {
    if (e.which == 13) {
        sendMessage();
    }
});
function messageOptions(msgId) {
    $(`#message-option-list-${msgId}`).css('display', 'block');
    return false;
}
function DeleteMessage(msgId) {
    Swal.fire({
        toast: true,
        title: 'میخواهید این پیام را حذف کنید؟',
        showDenyButton: true,
        confirmButtonText: 'بله',
        denyButtonText: `خیر`,
    }).then((result) => {
        if (result.isConfirmed) {
            connection.invoke('DeleteMessage', CHATID, msgId);
        }
    })
}
connection.on('ShowResult', function (res) {
    if (res.isSuccess) {
        Swal.fire({
            title: 'موفقیت',
            text: res.error,
            icon: 'success',
            timer: 1000,
            showConfirmButton: false
        });
    } else {
        Swal.fire({
            title: 'خطا',
            text: res.error,
            icon: 'error'
        });
    }
});