using SubredditTracker.Domain.Interfaces;

namespace SubredditTracker.API.Models
{
    public class TopPost : ITopPost
    {
        public required string PostTitle { get; set; }
        public int UpvoteCount { get; set; }
        public required string PostUrl { get; set; }
        
    }
}

