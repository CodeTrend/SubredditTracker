namespace SubredditTracker.Domain.Interfaces
{
    public interface ITopUser
    {
		public string User { get; set; }
        public int PostCount { get; set; }
    }
}

