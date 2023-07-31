# Subreddit Tracker

## Overview
Subreddit Tracker helps to track the most up voted posts in a given subreddit.

## Feature

- Start Tracking a Subreddit
- Stop Tracking a Subreddit
- Automatic update of lated upvotes via Signalr. No need to refresh your page.

## Applicaiton Screenshot
![Subreddit Tracker](https://github.com/CodeTrend/SubredditTracker/blob/main/application-demo-image.png)

## Project 

[Client Web App]<------signalr------>[API + BackgroundService]-----polling---->[Reddit API]


## Installation
1. Update the value for reddit ApplicationId, ApplicationSecret in the below file
\SubredditTracker\SubredditTracker.API\appsettings.json
2. Run the application.
3. Enter the "subreddit name" you wanted to track. And click on "Start Tracking"
 
## FAQ
Why it is giving a popup with error message?
1. Recheck your applicationsettings.json for reddit credentials
2. Reddit might ban some subreddits, try a different one.

