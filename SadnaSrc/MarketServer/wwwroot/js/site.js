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

    function extractQuery(param1,param2) {
        var queryValue = getParameterValues(param1);
        if (queryValue === undefined) {
            queryValue = getParameterValues(param2);
        }
        return queryValue;
    }

    socket.connectionMethods.onConnected = () => {
        console.log('client has been connected!');
        socketId = socket.connectionId;
        console.log('your SocketId is : ' + socketId);
        var systemId = extractQuery('systemId','SystemId');
        console.log('your systemId is : ' +systemId);
        if (systemId === undefined || systemId === 0) {
            socket.invoke('EnterSystem', socketId);
        } else {
            var status = extractQuery('status', 'Status');
            console.log('your status is : ' + status);
            if (status !== 'Guest') {
                socket.invoke('SubscribeSocket', systemId, socketId);
            }
        }
    }

    socket.connectionMethods.onDisconnected = () => {
        var status = extractQuery('status', 'Status');
        if (status !== 'Guest') {
            socket.invoke('UnSubscribeSocket', socketId);
        }
    }

    socket.clientMethods['IdentifyClient'] = (userId) => {
        location.href = window.location.href + '?systemId=' + userId + '&state=Guest';
    }

    socket.clientMethods['NotifyFeed'] = (feedMessage) => {
        var feedBox = $(
            "<div class='marketFeed'><span class='closebtn' onclick=\"this.parentElement.style.display = 'none';\">&times;</span>" +
            feedMessage +
            "</div>");
        $('#feedContainer').append(feedBox);
    }

    socket.start();

})