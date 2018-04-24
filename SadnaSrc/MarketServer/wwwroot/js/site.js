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
        $('#user-name-entry').val('');
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
        console.log('clicking!!!');
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

    socket.clientMethods['GetApiAnswer'] = (answer) => {
        console.log(answer);
    }

    socket.clientMethods['LoggingMarket'] = (statusCode, message,userId) => {
        console.log(statusCode);
        console.log(message);
        location.href = 'BrowseMarket' + '?SystemId=' + userId;
    }

    socket.start();

    var $button = document.getElementById('submit-signup-button');
    if ($button !== undefined && $button !== null) {
        $button.onclick = function () { submitSignUp(); }
    }

})