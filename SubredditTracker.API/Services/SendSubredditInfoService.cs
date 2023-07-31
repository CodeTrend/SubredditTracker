using Microsoft.AspNetCore.SignalR;
using SubredditTracker.Domain.Interfaces;
using SubredditTracker.API.Interfaces;
using SubredditTracker.API.Hubs;
using Esendex.TokenBucket;

namespace SubredditTracker.API.Services
{
    public class SendSubredditInfoService : BackgroundService
    {
        private readonly IHubContext<TrackingHub, ITrackingHubClient> _hubContext;
        private readonly ILogger<SendSubredditInfoService> _logger;
        private readonly ICachingService _cache;
        private readonly IRedditDataService _reddit;
        private const int _generalDelay = 1 * 10 * 1000;
        private const int _noOfPostsToLoad = 25;

        public SendSubredditInfoService(IHubContext<TrackingHub, ITrackingHubClient> hubContext, IRedditDataService reddit, ICachingService cache, ILogger<SendSubredditInfoService> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _reddit = reddit ?? throw new ArgumentNullException(nameof(reddit));
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bucket = TokenBuckets.Construct()
                  .WithCapacity(1)
                  .WithFixedIntervalRefillStrategy(1, TimeSpan.FromSeconds(1))
                  .Build();

            while (!stoppingToken.IsCancellationRequested)
            {


                await Task.Delay(_generalDelay, stoppingToken);
                await BroadcastSubredditInfo(stoppingToken, bucket);
            }
        }

        private async Task<string> BroadcastSubredditInfo(CancellationToken stoppingToken, ITokenBucket bucket)
        {
            if (_cache.TryGetValue(SubredditTracker.API.Utils.CacheConstants.SubRedditKey, out string subreddit))
            {
                bucket.Consume(1);
                Task<IEnumerable<ITopPost>> taskPost = Task.Run(async () => { return await _reddit.GetTopUpvotedPostAsync(subreddit, _noOfPostsToLoad, stoppingToken); });
                _ = taskPost.ContinueWith(async (posts) =>
                {
                    if (posts.Status == TaskStatus.RanToCompletion)
                    {
                        await _hubContext.Clients.All.PostsReceived(posts.Result);
                    }
                    else
                    {
                        await _hubContext.Clients.All.Error(posts == null || posts.Exception == null? "Application Error. Please check your configuration.": posts.Exception.Message);
                    }

                });
            }
            else
            {
                _logger.LogInformation("No Subreddits!!!");
            }

            return await Task.FromResult("Done");
        }
    }

}