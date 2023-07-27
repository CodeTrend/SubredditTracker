using System;
using Microsoft.Extensions.Options;

namespace SubredditTracker.API.Models
{
    public class RedditOptions : IOptions<RedditOptions>
    {
   
        public RedditOptions Value => this;

        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        public Uri RedirectUri { get; set; }
    }
}
