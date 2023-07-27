using SubredditTracker.Domain.Interfaces;

namespace SubredditTracker.API.Models
{
    public class TopUser : ITopUser
    {
        public required string User { get; set; }
        public int PostCount { get; set; }
    }
}

