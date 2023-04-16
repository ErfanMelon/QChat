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

function CreateGroup(event) {
    event.preventDefault();
    $("#NewChatModal .btn-primary").attr("disabled", true);
    var formData = new FormData();
    formData.append("Title", event.target[1].value);
    if (event.target[2].files[0]) {
        formData.append("Image", event.target[2].files[0]);
    }
    $.ajax({
        type: "POST",
        url: '@Url.Action("CreateGroup", "Home")',
        contentType: false,
        processData: false,
        enctype: "multipart/form-data",
        data: formData,
        success: function (data) {

            if (data.isSuccess == true) {

                $('#chatList').append(`<li data-chatid="${data.value}" onclick="LoadChat('${data.value}')">
                    <label>${event.target[1].value}</label>
                    <img src="img/Default.jpg" />
                    <span>  </span>
                </li>`);
                $('#NewChatModal').modal('hide');
                $('#NewChatModal #GroupImage').val('');
                $('#NewChatModal #txtGroupTitle').val('');
            }
            else {

                alert(data.error);
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }

    });
    $('#NewChatModal .btn-primary').prop("disabled", false);
}

function CreatePrivateChat(event) {
    event.preventDefault();
    var formData = new FormData();
    formData.append("UserId", event.target[1].value);
    formData.append("UserName", event.target[2].value);
    $.ajax({
        type: "POST",
        url: '@Url.Action("CreatePrivateChat", "Home")',
        contentType: false,
        processData: false,
        enctype: "multipart/form-data",
        data: formData,
        success: function (data) {

            if (data.isSuccess == true) {

                $('#chatList').append(`<li data-chatid="${data.value}" onclick="LoadChat('${data.value}')">
                    <label>${event.target[2].value}</label>
                    <img src="img/Default.jpg" />
                    <span>  </span>
                </li>`);
                $('#NewPrivateChatModal').modal('hide');
                $('#NewPrivateChatModal #UserName').val('');
            }
            else {

                alert(data.error);
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
$("#ChatOption").on('click', function () {
    $('#ChatInfoModal #members').html('');
    var selectedchatId = $(this).attr("data-chatId");
    $('#ChatInfoModal .input-group #ChatId').val(selectedchatId);
    // Get Chat Data And Show To ChatInfoModal
    $.ajax({
        type: "POST",
        url: '@Url.Action("GetChatInfo", "Home")',
        data: { chatId: selectedchatId },
        success: function (data) {
            if (data.isSuccess) {
                for (i in data.value.members) {
                    $('#ChatInfoModal #members').append(`<h5 class="mb-2">${data.value.members[i]}</h5>`)
                }
                $('#ChatInfoModal').modal('show');
            }
            else {
                alert(data.error);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
});
function AddUserToGroup(event) {
    event.preventDefault();
    var formData = {
        UserName: event.target[2].value,
        ChatId: event.target[1].value
    };
    $.ajax({
        type: "POST",
        url: '@Url.Action("AddUserToGroup","Home")',
        data: formData,
        success: function (data) {
            if (data.isSuccess) {
                $('#ChatInfoModal').modal('hide');
                $('#ChatInfoModal #UserName').val('');
            }
            else {
                alert(data.error);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}
function LoadChat(id) {
    $.ajax({
        type: 'POST',
        url: '@Url.Action("GetChat", "Home")',
        data: { chatId: id, oldChatId: CHATID, connectionId: sessionId },
        success: function (data) {
            if (data.isSuccess) {
                CHATID = id;
                $('#currentChat .chats').html(''); // clear old chats
                $('#ChatOption').attr('data-chatId', id); // change chatId
                $('#currentChat').css('display', 'block'); // show current chat page
                if (data.value.imageSrc) { // change chat image
                    $('#currentChat .header img').attr('src', `/${data.value.imageSrc}`);
                }
                else {
                    $('#currentChat .header img').attr('src', "img/Default.jpg");
                }
                $('#currentChat .header h2').text(data.value.title); // change chat title
                for (i in data.value.messages) {
                    if (data.value.messages[i].username === UserName) {
                        $('#currentChat .chats').append(`
                <div class="chat-me row">
                                <div class="chat col-2" oncontextmenu="messageOptions('${data.value.messages[i].id}')">
                                    <small class="text-white">${data.value.messages[i].username}</small>
                                    <p>${data.value.messages[i].content}</p>
                                    <span>${data.value.messages[i].postDate}</span>
                                </div>
                                <div class="col-10" style="display:none" id="message-option-list-${data.value.messages[i].id}">
                                    <button class="btn btn-danger" onclick="DeleteMessage('${data.value.messages[i].id}')"> حذف </button>
                                    <button class="btn btn-white" onclick="$('#message-option-list-${data.value.messages[i].id}').css('display','none')">بستن</button>
                                </div>
                </div>`);
                    }
                    else {
                        $('#currentChat .chats').append(`
                <div class="chat-you row">
                                <div class="chat col-2">
                                    <small>${data.value.messages[i].username}</small>
                                    <p>${data.value.messages[i].content}</p>
                                    <span>${data.value.messages[i].postDate}</span>
                                </div>
                                <div class="col-10" style="display:none" id="message-option-list-${data.value.messages[i].id}">
                                    <button class="btn btn-danger" onclick="DeleteMessage('${data.value.messages[i].id}')"> حذف </button>
                                    <button class="btn btn-white" onclick="$('#message-option-list-${data.value.messages[i].id}').css('display','none')">بستن</button>
                                </div>
                </div>`);
                    }
                    $(`#chatList li[data-chatid="${id}"] span`).text(data.value.messages[i].postDate);
                }
                $('#currentChat .chat:last').attr('id', 'LastMessageInThisChat');
                document.getElementById('LastMessageInThisChat').scrollIntoView();
                $('#currentChat .chat:last').removeAttr('id');
            }
            else {
                alert(data.error);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
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
connection.on('SendNotification', function (name, chatId, msg) {
    if (CHATID !== chatId) {
        if (Notification.permission === 'granted') {
            var chatTitle = $(`#chatList li[data-chatid="${chatId}"] label`).text();
            var notification = new Notification(chatTitle,
                {
                    body: `${name} : ${msg}`
                });
        }
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