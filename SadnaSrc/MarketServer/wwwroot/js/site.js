// Write your JavaScript code.
$(document).ready(function() {
    var connection = new WebSocketManager.Connection('ws://localhost:3000/market');
    connection.enableLogging = true;

    connection.connectionMethods.onConnected = () => {
        console.log('client has been connected!');
        connection.invoke('EnterSystem', connection.connectionId, message);
    }

    connection.connectionMethods.onDisconnected = () => {
        console.log('client has been disconnected!');
    }

    connection.clientMethods['identifyClient'] = (userId) => {
        var messageText = 'your user id is:' + userId;
        console.log(userId);
    }
    connection.start();

})