namespace SubredditTracker.Domain.Interfaces
{
    public interface ITopPost
	{
        public string PostTitle { get; set; }
        public int UpvoteCount { get; set; }
    }
}

