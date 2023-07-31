using SubredditTracker.Domain.Interfaces;

namespace SubredditTracker.API.Interfaces
{
	public interface IRedditDataService
	{
        public Task<IEnumerable<ITopPost>> GetTopUpvotedPostAsync(string subreddit, int postCount, CancellationToken cancellationToken);

    }
}

