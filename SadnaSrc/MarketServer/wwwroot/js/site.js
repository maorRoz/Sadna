// Write your JavaScript code.
$(document).ready(function() {
    var socket = new WebSocketManager.Connection('ws://localhost:3000/market');
    socket.enableLogging = true;

    socket.connectionMethods.onConnected = () => {
        console.log('client has been connected!');
        socket.invoke('EnterSystem', socket.connectionId);
    }

    socket.connectionMethods.onDisconnected = () => {
    }

    socket.clientMethods['identifyClient'] = (userId) => {
        var messageText = 'your user id is:' + userId;
        console.log(messageText);
    }
    socket.start();

})