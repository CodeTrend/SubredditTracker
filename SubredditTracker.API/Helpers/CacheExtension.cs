namespace SubredditTracker.API.Helpers
{
    using SubredditTracker.API.Services;
    using SubredditTracker.Domain.Interfaces;

    public static class CacheExtension
    {
        public static IServiceCollection ConfigureCache(this IServiceCollection service)
        {
            service.AddMemoryCache();
            return service.AddSingleton<ICachingService, CachingService>();
        }

    }
}