namespace SubredditTracker.API.Interfaces
{
    using SubredditTracker.Domain.Interfaces;

    public interface ITrackingHubClient
	{
        Task PostsReceived(IEnumerable<ITopPost> message);
        Task UsersReceived(IEnumerable<ITopUser> message);
        Task Error(string message);
    }
}

