﻿// Write your JavaScript code.
$(document).ready(function() {
    var socketUrl = "ws://" + window.location.host + "/market";
    var socket = new WebSocketManager.Connection(socketUrl);
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

    function identifyClient(userId) {
        const systemId = extractQuery('systemId', 'SystemId');
        if (systemId === undefined || (systemId === '0' && userId !== 0)) {
            location.href = window.location.href.substring(0, window.location.href.indexOf('?')) +
                '?systemId=' +
                userId +
                '&state=Guest';
        }
    }

    socket.connectionMethods.onConnected = () => {
        console.log('client has been connected!');
        socketId = socket.connectionId;
        console.log('your SocketId is : ' + socketId);
        var systemId = extractQuery('systemId','SystemId');
        console.log('your systemId is : ' + systemId);
        if (systemId === undefined || systemId === 0 || systemId === '0') {
            $.get("http://"+window.location.host + "/api/enter",
                function(data) {
                    identifyClient(data);
                });
        } else {
            var state = extractQuery('state', 'State');
            console.log('your state is : ' + state);
            if (state !== 'Guest' && systemId !== 0) {
                socket.invoke('SubscribeSocket', systemId, socketId);
            }
        }
    }

    socket.connectionMethods.onDisconnected = () => {
    }

    socket.clientMethods['NotifyFeed'] = (feedMessage) => {
        var feedBox = $(
            "<div class='marketFeed'><span class='closebtn' onclick=\"this.parentElement.style.display = 'none';\">&times;</span>" +
            feedMessage +
            "</div>");
        $('#feedContainer').append(feedBox);
    }

    socket.start();

    $(window).on("unload", function (e) {
        var state = extractQuery('state', 'State');
        if (state === 'Registered' || state === 'Admin') {
            socket.invoke('UnSubscribeSocket', socketId);
         }
    });
})