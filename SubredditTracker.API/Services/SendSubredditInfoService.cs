using System;
using Microsoft.AspNetCore.SignalR;
using SubredditTracker.Domain.Interfaces;
using SubredditTracker.API.Interfaces;
using SubredditTracker.API.Hubs;
using SubredditTracker.API.Models;
using SubredditTracker.Domain;
using System.Threading;

namespace SubredditTracker.API.Services
{
    public class SendSubredditInfoService : BackgroundService
    {
        private readonly IHubContext<TrackingHub, ITrackingHubClient> _hubContext;
        private readonly ILogger<SendSubredditInfoService> _logger;
        private readonly ICachingService _cache;
        private readonly IRedditDataService _reddit;
        public SendSubredditInfoService(IHubContext<TrackingHub, ITrackingHubClient> hubContext, IRedditDataService reddit, ICachingService cache, ILogger<SendSubredditInfoService> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _reddit = reddit ?? throw new ArgumentNullException(nameof(reddit));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(2000);
            if (_cache.TryGetValue(SubredditTracker.API.Utils.CacheConstants.SubRedditKey, out string subreddit))
            {

                while (!stoppingToken.IsCancellationRequested)
                {
                    //var posts = await _reddit.GetTopUpvotedPostAsync(subreddit, 5, stoppingToken);
                    //TODO:


                    IEnumerable<ITopPost> posts = new List<TopPost>
                    {
                        new TopPost() { PostTitle = "Hello", UpvoteCount = 10 },
                        new TopPost() { PostTitle = "Bye", UpvoteCount = 11 }
                    };

                    await _hubContext.Clients.All.PostsReceived(posts);
                }
            }
            else
            {
                _logger.LogInformation("No Subreddits!!!");
            }
            await Task.Delay(15000);
        }
    }
}

