using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Reddit;
using SubredditTracker.API.Interfaces;
using SubredditTracker.API.Models;
using SubredditTracker.Domain;
using SubredditTracker.Domain.Interfaces;
//using Reddit.AuthTokenRetriever;
//using Reddit.Controllers;
//using Reddit.Inputs;
//using Reddit.Inputs.Search;
//using Reddit.NET.Client;
//using RedditAlerts.Models;

namespace SubredditTracker.API.Services
{
	public class RedditDataService : IRedditDataService
    {
        private readonly IApiAuthenticator _redditAuthenticator;
        private readonly ICachingService _cachingService;
        private static readonly Guid _accessTokenKey = Guid.NewGuid();
        private readonly ILogger<RedditDataService> _logger;

        //TODO: remove this and use Reddit.NET
        private readonly HttpClient _httpClient;

        public RedditDataService(HttpClient httpClient, IApiAuthenticator authenticator, ICachingService cachingService, ILogger<RedditDataService> logger)
        {
            _cachingService = cachingService ?? throw new ArgumentNullException(nameof(cachingService));
            _redditAuthenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        }

        private async void UpdateAccessToken(CancellationToken cancellationToken)
        {
            if (!_cachingService.TryGetValue(_accessTokenKey, out RedditToken accessToken))
            {
                accessToken = await GetAndSaveToken(cancellationToken);
            }
            else
            {
                _logger.LogInformation("Access token retrieved from cache");
                
            }
            SetHeaders(accessToken.Value);
            //return accessToken.Value;
        }

        public async Task<IEnumerable<ITopPost>> GetTopUpvotedPostAsync(string subreddit, int postCount, CancellationToken cancellationToken)
        {
            UpdateAccessToken(cancellationToken);
            var url = $"r/{subreddit}/hot?limit={postCount.ToString()}";
            var popularResponse = await _httpClient.GetAsync(url);
            if (popularResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                UpdateAccessToken(cancellationToken);
                popularResponse = await _httpClient.GetAsync(url);
                popularResponse.EnsureSuccessStatusCode();
            }

            var getResponseContent = await popularResponse.Content.ReadAsStringAsync();

            return new List<TopPost>() { };

        }


      
        //public IEnumerable<ITopUser> GetTopUser()
        //{

        //}

        //public  Task<string> GetPostsAsync(int postsCount, CancellationToken cancellationToken)
        //{
        //    if (!_cachingService.TryGetValue(_accessTokenKey, out RedditToken accessToken))
        //    {
        //        accessToken = await GetAndSaveToken(cancellationToken);
        //    }
        //    else
        //    {
        //        _logger.LogInformation("Access token retrieved from cache");
        //    }

        //    SetHeaders(accessToken.Value);

        //    var url = $"r/funny/top?limit={postsCount}";
        //    var popularResponse = await _httpClient.GetAsync(url);
        //    if (popularResponse.StatusCode == HttpStatusCode.Unauthorized)
        //    {
        //        accessToken = await GetAndSaveToken(cancellationToken);
        //        SetHeaders(accessToken.Value);
        //        popularResponse = await _httpClient.GetAsync(url);
        //        popularResponse.EnsureSuccessStatusCode();
        //    }

        //    var getResponseContent = await popularResponse.Content.ReadAsStringAsync();
        //    //var result = JsonSerializer.Deserialize<RedditApiResponse>(getResponseContent,
        //    //    new JsonSerializerOptions
        //    //    {
        //    //        PropertyNameCaseInsensitive = true,
        //    //    });
        //    //var redditPosts = result.Data.Children;
        //    //var posts = redditPosts.Select(redditPost => RedditPostMapper.Map(redditPost.Data))
        //    //                       .ToList();
        //    //return Task;
        //    return await Task.FromResult<string>("test");
        //}

        private void SetHeaders(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private async Task<RedditToken> GetAndSaveToken(CancellationToken cancellationToken)
        {
            var accessToken = await _redditAuthenticator.GetAccessToken(cancellationToken);
            _logger.LogInformation("New access token retrieved from api");
            var validUntil = DateTime.UtcNow.AddSeconds(accessToken.ExpiredInSeconds);
            _cachingService.Set(_accessTokenKey, validUntil, accessToken);
            return accessToken;
        }

        



        //private readonly RedditClient _redditClient;
        //public RedditService(string appID, string appSecret)
        //{
        //    _redditClient = AuthorizeUser(appID, appSecret);
        //}

        //public RedditService(string appID, string appSecret, string refreshToken)
        //{
        //    _redditClient = new RedditClient(appID, refreshToken, appSecret);
        //}

        //public RedditClient AuthorizeUser(string appId, string appSecret, int port = 8080)
        //{
        //    AuthTokenRetrieverLib authTokenRetrieverLib = new(appId, port, "localhost",
        //        appSecret: appSecret);

        //    // Start the callback listener.  --Kris
        //    // Note - Ignore the logging exception message if you see it.  You can use Console.Clear() after this call to get rid of it if you're running a console app.
        //    authTokenRetrieverLib.AwaitCallback();

        //    OpenBrowser(authTokenRetrieverLib.AuthURL());
        //    while (string.IsNullOrWhiteSpace(authTokenRetrieverLib.RefreshToken))
        //    {

        //    }
        //    authTokenRetrieverLib.StopListening();
        //    return new(appId, authTokenRetrieverLib.RefreshToken, appSecret,
        //        authTokenRetrieverLib.AccessToken);
        //}

        //private static void OpenBrowser(string authUrl, string browserPath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe")
        //{
        //    try
        //    {

        //        Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
        //        //ProcessStartInfo processStartInfo = new ProcessStartInfo(authUrl);
        //        //Process.Start(processStartInfo);
        //    }
        //    catch (System.ComponentModel.Win32Exception)
        //    {
        //        // This typically occurs if the runtime doesn't know where your browser is.  Use BrowserPath for when this happens.  --Kris
        //        ProcessStartInfo processStartInfo = new ProcessStartInfo(browserPath)
        //        {
        //            Arguments = authUrl
        //        };
        //        Process.Start(processStartInfo);
        //    }
        //}

        //public List<Post> SearchPosts(string searchString, string subReddit = "all")
        //{
        //    List<Post> posts;
        //    if (string.IsNullOrWhiteSpace(subReddit))
        //    {
        //        subReddit = "all";
        //    }
        //    posts = _redditClient.Subreddit(subReddit)
        //            .Search(new SearchGetSearchInput(searchString));
        //    foreach (Post post in posts)
        //    {
        //        DigestedRedditPost dig = new(post);
        //        dig.Title = post.Title;
        //    }
        //    return posts;
        //}

        //public List<DigestedRedditPost> GetLastHoursFilteredPosts(string subReddit,
        //    List<string> keywords, string? after = "")
        //{
        //    List<DigestedRedditPost> digestedRedditPosts = new();
        //    List<Post> posts = _redditClient.Subreddit
        //        (subReddit).Posts.GetNew(new
        //        CategorizedSrListingInput(after: after, limit: 100));
        //    int postsAfterHour = 0; // sometimes Reddit time travels so we give them 3 strikes
        //    foreach (Post post in posts)
        //    {
        //        if (post.Created < DateTime.Now.AddHours(-1))
        //        {
        //            postsAfterHour += 1;
        //            if (postsAfterHour > 3)
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            if (keywords.Any(i => post.Title.Contains(i,
        //                StringComparison.OrdinalIgnoreCase))
        //                || (string.IsNullOrWhiteSpace(
        //                    post.Listing.SelfText) == false
        //                    && keywords.Any(i =>
        //                    post.Listing.SelfText.Contains(i,
        //                    StringComparison.OrdinalIgnoreCase))))
        //            {
        //                digestedRedditPosts.Add(new(post));
        //            }
        //        }
        //    }
        //    if (posts.Count == 100
        //        && posts.Last().Created > DateTime.Now.AddHours(-1))
        //    {
        //        digestedRedditPosts.AddRange(
        //            GetLastHoursPosts(subReddit, posts.Last().Fullname));
        //    }
        //    return digestedRedditPosts;
        //}

        //public List<DigestedRedditPost> GetLastHoursPosts(string subReddit,
        //    string? after = "")
        //{
        //    List<DigestedRedditPost> digestedRedditPosts = new();
        //    List<Post> posts = _redditClient.Subreddit
        //        (subReddit).Posts.GetNew(new
        //        CategorizedSrListingInput(after: after, limit: 100));
        //    int postsAfterHour = 0; // sometimes Reddit time travels so we give them 3 strikes
        //    foreach (Post post in posts)
        //    {
        //        if (post.Created < DateTime.Now.AddHours(-1))
        //        {
        //            postsAfterHour += 1;
        //            if (postsAfterHour > 3)
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            digestedRedditPosts.Add(new(post));
        //        }
        //    }
        //    if (posts.Count == 100
        //        && posts.Last().Created > DateTime.Now.AddHours(-1))
        //    {
        //        digestedRedditPosts.AddRange(
        //            GetLastHoursPosts(subReddit, posts.Last().Fullname));
        //    }
        //    return digestedRedditPosts;
        //}

    }
}

