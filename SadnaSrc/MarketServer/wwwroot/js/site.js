// Write your JavaScript code.
$(document).ready(function() {
    var socket = new WebSocketManager.Connection('ws://localhost:3000/market');
    var socketId = null;
    socket.enableLogging = true;


    function getParameterValues(param) {
        var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
        for (var i = 0; i < url.length; i++) {
            var urlparam = url[i].split('=');
            if (urlparam[0] === param) {
                return urlparam[1];
            }
        }
    }

    function submitSignUp() {
        var nameEntry = $('#user-name-entry').val().trim();
        var addressEntry = $('#user-address-entry').val().trim();
        var passEntry = $('#user-password-entry').val().trim();
        var creditEntry = $('#user-creditcard-entry').val().trim();
        socket.invoke('SignUpUser',
            socketId,
            getParameterValues('SystemId'),
            nameEntry,
            addressEntry,
            passEntry,
            creditEntry);
        console.log(socketId);
        console.log(getParameterValues('SystemId'));
        console.log(nameEntry);
        console.log(addressEntry);
        console.log(passEntry);
        console.log(creditEntry);
        console.log('clicking submit signup!!!');
    }

    function submitSignIn() {
        var nameEntry = $('#user-name-entry').val().trim();
        var passEntry = $('#user-password-entry').val().trim();
        socket.invoke('SignInUser',
            socketId,
            getParameterValues('SystemId'),
            nameEntry,
            passEntry);
        console.log(socketId);
        console.log(getParameterValues('SystemId'));
        console.log(nameEntry);
        console.log(passEntry);
        console.log('clicking submit signin!!!');
    }



    socket.connectionMethods.onConnected = () => {
        console.log('client has been connected!');
        socketId = socket.connectionId;
        console.log('your SocketId is : ' + socketId);
        var systemId = getParameterValues('SystemId');
        console.log('your SystemId is : ' +systemId);
        if (systemId === undefined || systemId === 0) {
            socket.invoke('EnterSystem', socketId);
        }
    }

    socket.connectionMethods.onDisconnected = () => {
    }

    socket.clientMethods['IdentifyClient'] = (userId) => {
        location.href = window.location.href + '?SystemId=' + userId;
    }

    socket.clientMethods['LoggingMarket'] = (statusCode, message,userId) => {
        console.log(statusCode);
        console.log(message);
        if (statusCode === 0) {
            var successMessage =
                $(
                    "<div class='success'><span class='closebtn' onclick=\"this.parentElement.style.display = 'none';\">&times;</span>" +
                    message +
                    "</div>");
            $('#alertContainer').append(successMessage);
            location.href = 'BrowseMarket' + '?SystemId=' + userId;
        } else {
            var alertMessage =
                $(
                    "<div class='error'><span class='closebtn' onclick=\"this.parentElement.style.display = 'none';\">&times;</span>" +
                    message +
                    "</div>");
            $('#alertContainer').append(alertMessage);
        }
    }

    /*<div class="alert">
    <span class="closebtn" onclick="this.parentElement.style.display = 'none';">&times;</span>
    This is an alert box.
</div>*/

    socket.clientMethods['GetApiAnswer'] = (answer) => {
        console.log(answer);
    }

    socket.start();

    var $submitSignupButton = document.getElementById('submit-signup-button');
    if ($submitSignupButton !== undefined && $submitSignupButton !== null) {
        $submitSignupButton.onclick = function () { submitSignUp(); }
    }

    var $submitSigninButton = document.getElementById('submit-signin-button');
    if ($submitSigninButton !== undefined && $submitSigninButton !== null) {
        $submitSigninButton.onclick = function () { submitSignIn(); }
    }

})