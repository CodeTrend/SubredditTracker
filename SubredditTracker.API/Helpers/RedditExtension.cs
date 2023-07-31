using SubredditTracker.API.Models;
using SubredditTracker.Domain.Interfaces;
using SubredditTracker.API.Services;
using SubredditTracker.API.Interfaces;

namespace SubredditTracker.API.Helpers
{
    public static class RedditExtension
    {
        public static void ConfigureRedditApiService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedditApiOptions>(configuration);

            var userAgent = configuration.GetValue<string>("UserAgent");
            services.AddHttpClient<IApiAuthenticator, RedditAuthenticatorService>(client =>
            {
                client.BaseAddress = new Uri("https://www.reddit.com/api/v1/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            });
            services.AddHttpClient<IRedditDataService, RedditDataService>(client =>
            {
                client.BaseAddress = new Uri("https://oauth.reddit.com/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            });
        }
    }
}

