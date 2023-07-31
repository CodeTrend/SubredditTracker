using System.Text.Json.Serialization;

namespace SubredditTracker.Domain
{
    public class RedditToken
    {
        [JsonPropertyName("access_token")]
        public string Value { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiredInSeconds { get; set; }
    }
}

