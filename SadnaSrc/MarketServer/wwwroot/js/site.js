// Write your JavaScript code.
$(document).ready(function() {
    var connection = new WebSocketManager.Connection('ws://localhost:3000/market');
    connection.enableLogging = true;

    connection.connectionMethods.onConnected = () => {
        console.log('client has been connected!');
    }

    connection.connectionMethods.onDisconnected = () => {
        console.log('client has been disconnected!');
    }
    connection.clientMethods['marketMessage'] = (socketId, message) => {
        var messageText = socketId + 'said : ' + message;
        $('#messages').append('<li>' + messageText +'</li>')
        console.log(messageText);
    }
    connection.start();

    var $messagecontent = $('#message-content');
    $messagecontent.keyup(function(e) {
        if (e.keyCode === 13) {
            var message = $messagecontent.val().trim();
            if (message.length === 0) {
                return false;
            }

            connection.invoke('SendMessage', connection.connectionId, message);
            $messagecontent.val('');
        }
    });
})