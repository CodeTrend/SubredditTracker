"use strict";
var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Debug).withUrl("https://localhost:7154/hub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;


connection.on("PostsReceived", function (message) {
    //var li = document.createElement("li");
    //document.getElementById("messagesList").appendChild(li);
    //// We can assign user-supplied strings to an element's textContent because it
    //// is not interpreted as markup. If you're assigning in any other way, you
    //// should be aware of possible script injection concerns.
    //li.textContent = `${user} says ${message}`;

    if (message && message.lenth > 0) {
        $('#tbl-subreddit-post').dataTable({
            "data": message,
            "columns": [{
                "data": "PostTitle"
            },
            {
                "data": "UpvoteCount"
            }]
        });
    }
    console.log("PostsReceived. Yay!!!");
    console.log(message);
});


connection.on("UsersReceived", function (message) {
    //var li = document.createElement("li");
    //document.getElementById("messagesList").appendChild(li);
    //// We can assign user-supplied strings to an element's textContent because it
    //// is not interpreted as markup. If you're assigning in any other way, you
    //// should be aware of possible script injection concerns.
    //li.textContent = `${user} says ${message}`;

    console.log("UsersReceived. Yay!!!");
    console.log(message);
});




connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
    console.log('subreddit tracker hub has connection started');

}).catch(function (err) {
    console.log('subreddit tracker hub has connection error');
    return console.error(err.toString());
});

jQuery(function () {
    jQuery('#root-container-result').hide();
    jQuery('#subredditToTrack').focus();
    Tracker.getAndSetTracking();
    jQuery('#start-tracking').click(Tracker.onStartTracking);
    jQuery('#stop-tracking').click(Tracker.onStopTracking);
});

//TODO: move TrackerConstants to index.cshtml and manage it from the appsetting.json
var TrackerConstants = {
    apiBaseUrl: 'https://localhost:7154/api'
}

var Tracker = {
    getSubreddit: function () {
        return jQuery('#subredditToTrack').val()
    },
    getAndSetTracking: function () {
        var subreddit = Tracker.getSubreddit();
        jQuery.get(TrackerConstants.apiBaseUrl + '/subreddit' + subreddit, function (data) {
            jQuery('#subredditToTrack').val(data);
            jQuery('#subredditToTrack').prop("readonly", true);
        });
    },
    onStartTracking: function () {
        var subreddit = Tracker.getSubreddit();
        jQuery.get(TrackerConstants.apiBaseUrl + '/subreddit/subscribe/' + subreddit, function (data) {
            jQuery('#subredditToTrack').prop("readonly", true);
            console.log(subreddit + '  ' + data);
            jQuery('#root-container-result').show();
        });
    },
    onStopTracking: function () {
        var subreddit = Tracker.getSubreddit();
        jQuery.get(TrackerConstants.apiBaseUrl + '/subreddit/unsubscribe/' + subreddit, function (data) {
            jQuery('#subredditToTrack').val('');
            jQuery('#subredditToTrack').prop("readonly", false);
            jQuery('#subredditToTrack').focus();
            console.log(subreddit + '  ' + data);
            jQuery('#root-container-result').hide();
        })
    }
}
