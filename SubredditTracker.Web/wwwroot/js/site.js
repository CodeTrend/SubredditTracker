"use strict";
var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Debug).withUrl("https://localhost:7154/hub").build();
connection.on("PostsReceived", function (message) {
    if (message && message.length > 0) {
        jQuery('#tbl-subreddit-post').dataTable().fnClearTable();
        jQuery('#tbl-subreddit-post').dataTable().fnAddData(message);
    }
});
connection.on("Error", function (message) {
    alert(message);
});
connection.on("UsersReceived", function (message) {
    if (message && message.length > 0) {
        jQuery('#tbl-subreddit-users').dataTable().fnClearTable();
        jQuery('#tbl-subreddit-users').dataTable().fnAddData(message);
    }
});




connection.start().then(function () {

}).catch(function (err) {
    return console.error(err.toString());
});

jQuery(function () {
    jQuery('#root-container-result').hide();
    jQuery('#subredditToTrack').focus();
    Tracker.getAndSetTracking();
    jQuery('#start-tracking').click(Tracker.onStartTracking);
    jQuery('#stop-tracking').click(Tracker.onStopTracking);


    jQuery('#tbl-subreddit-post').dataTable({
        "data": [],
        info: false,
        ordering: false,
        paging: false,
        searching: false,
        bAutoWidth: false,
        "language": {
            "emptyTable": "Post will refresh automatically.."
        },
        "columns": [{
            "name": "Post Title",
            "data": "postTitle",
            "render": function (data, type, row, meta) {
                if (type === 'display') {
                    data = '<a href="' + row["postUrl"] + '">' + data + '</a>';
                }

                return data;
            }
        },
        {
            "name": "Upvote Count",
            "data": "upvoteCount"
            }], "fnInitComplete": function () {

            $("#tbl-subreddit-post").css("width", "100%");
            }
    });

    jQuery('#tbl-subreddit-users').dataTable({
        "data": [],
        info: false,
        ordering: false,
        paging: false,
        searching: false,
        bAutoWidth: false,
        "language": {
            "emptyTable": "User list will start to refresh automatically.."
        },
        "columns": [{
            "name": "User",
            "data": "user",
        },
        {
            "name": "Post Count",
            "data": "postCount"
        }]
    });
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
            jQuery('#root-container-result').show();
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
