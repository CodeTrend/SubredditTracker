namespace SubredditTracker.Domain.Interfaces
{
    public interface IApiAuthenticator
    {
        Task<RedditToken> GetAccessToken(CancellationToken cancellationToken);
    }
}

