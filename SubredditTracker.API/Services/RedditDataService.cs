
namespace SubredditTracker.API.Services
{
    using System.Net;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;
    using Interfaces;
    using Models;
    using Domain;
    using SubredditTracker.Domain.Interfaces;

    public class RedditDataService : IRedditDataService
    {
        private readonly IApiAuthenticator _redditAuthenticator;
        private readonly ICachingService _cachingService;
        private static readonly Guid AccessTokenKey = Guid.NewGuid();
        private readonly ILogger<RedditDataService> _logger;
        private readonly HttpClient _httpClient;

        public RedditDataService(HttpClient httpClient, IApiAuthenticator authenticator, ICachingService cachingService, ILogger<RedditDataService> logger)
        {
            _cachingService = cachingService ?? throw new ArgumentNullException(nameof(cachingService));
            _redditAuthenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        }

        private async Task UpdateAccessToken(CancellationToken cancellationToken)
        {
            if (!_cachingService.TryGetValue(AccessTokenKey, out RedditToken accessToken))
            {
                accessToken = await GetAndSaveToken(cancellationToken);
            }
            else
            {
                _logger.LogInformation("Access token retrieved from cache");

            }
            SetHeaders(accessToken.Value);
        }

        public async Task<IEnumerable<ITopPost>> GetTopUpvotedPostAsync(string subreddit, int postCount, CancellationToken cancellationToken)
        {
            await UpdateAccessToken(cancellationToken);
            var url = $"r/{subreddit}/top?limit={postCount}&t=all";
            var popularResponse = await _httpClient.GetAsync(url, cancellationToken);
            if (popularResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                await UpdateAccessToken(cancellationToken);
                popularResponse = await _httpClient.GetAsync(url, cancellationToken);
                popularResponse.EnsureSuccessStatusCode();
            }

            var redditResponseContent = await popularResponse.Content.ReadAsStringAsync(cancellationToken);
            var topPosts = JsonConvert.DeserializeObject<RedditApiResponse>(redditResponseContent);
            if (topPosts != null && topPosts.Data != null)
            {
                var posts = topPosts.Data.Children.Select(p => new TopPost() { PostTitle = p.Data.Title, UpvoteCount = (int)p.Data.Ups, PostUrl = p.Data.Url });
                return posts;
            }
            throw new Exception("Response from Reddit API. Try a different subreddit :", new Exception(redditResponseContent));
        }

        private void SetHeaders(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private async Task<RedditToken> GetAndSaveToken(CancellationToken cancellationToken)
        {
            var accessToken = await _redditAuthenticator.GetAccessToken(cancellationToken);
            _logger.LogInformation("New access token retrieved from api");
            var validUntil = DateTime.UtcNow.AddSeconds(accessToken.ExpiredInSeconds);
            _cachingService.Set(AccessTokenKey, validUntil, accessToken);
            return accessToken;
        }


    }
}

