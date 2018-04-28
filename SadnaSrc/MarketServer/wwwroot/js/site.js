﻿// Write your JavaScript code.
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
            getParameterValues('systemId'),
            nameEntry,
            addressEntry,
            passEntry,
            creditEntry);
        $('#user-name-entry').val('');
        $('#user-address-entry').val('');
        $('#user-password-entry').val('');
        $('#user-creditcard-entry').val('');
    }

    function submitSignIn() {
        var nameEntry = $('#user-name-entry').val().trim();
        var passEntry = $('#user-password-entry').val().trim();
        socket.invoke('SignInUser',
            socketId,
            getParameterValues('systemId'),
            nameEntry,
            passEntry);
        $('#user-name-entry').val('');
        $('#user-password-entry').val('');
    }



    socket.connectionMethods.onConnected = () => {
        console.log('client has been connected!');
        socketId = socket.connectionId;
        console.log('your SocketId is : ' + socketId);
        var systemId = getParameterValues('systemId');
        console.log('your systemId is : ' +systemId);
        if (systemId === undefined || systemId === 0) {
            socket.invoke('EnterSystem', socketId);
        } else if (getParameterValues('state') !== 'Guest') {
              var $signUpRemove = document.getElementById('signUpPage');
              $signUpRemove.parentNode.removeChild($signUpRemove);
              var $signInRemove = document.getElementById('signInPage');
              $signInRemove.parentNode.removeChild($signInRemove);
        }
    }

    socket.connectionMethods.onDisconnected = () => {
    }

    socket.clientMethods['IdentifyClient'] = (userId) => {
        location.href = window.location.href + '?systemId=' + userId + '&state=Guest';
    }

    socket.clientMethods['LoggedMarket'] = (message, userId, state) => {
            location.href = 'MainLobby' + '?systemId=' + userId + '&state=' + state +'&message=' +message; 
    }

    socket.clientMethods['NotifyFeed'] = (feedMessage) => {
        var feedBox = $(
            "<div class='marketFeed'><span class='closebtn' onclick=\"this.parentElement.style.display = 'none';\">&times;</span>" +
            feedMessage +
            "</div>");
        $('#feedContainer').append(feedBox);
    }

    socket.clientMethods['ErrorApi'] = (error) => {
        console.log(error);
        var alertBox =
            $("<div class='error'><span class='closebtn' onclick=\"this.parentElement.style.display = 'none';\">&times;</span>" +
                error +
                "</div>");
        $('#alertContainer').append(alertBox);
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