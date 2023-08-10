
namespace SubredditTracker.API.Models
{
    using Newtonsoft.Json;

    internal class RedditApiResponse
    {
        public RedditApiData Data { get; set; }
    }

    internal class RedditApiData
    {
        public List<RedditPost> Children { get; set; }
    }

    internal class RedditPost
    {
        public RedditPostData Data { get; set; }
    }

    internal class RedditPostData
    {
        [JsonProperty("title")] public string Title { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("ups")] public int Ups { get; set; }
    }
}
