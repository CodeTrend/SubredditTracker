﻿using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using SubredditTracker.Domain.Interfaces;
using SubredditTracker.API.Models;
using SubredditTracker.Domain;

[assembly: InternalsVisibleTo("UnitTests")]
namespace SubredditTracker.API.Services
{
    internal class RedditAuthenticatorService : IApiAuthenticator
    {
        private readonly RedditApiOptions _redditOptions;
        private readonly HttpClient _httpClient;

        public RedditAuthenticatorService(
            IOptions<RedditApiOptions> options,
            HttpClient httpClient)
        {
            _redditOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<RedditToken> GetAccessToken(CancellationToken cancellationToken)
        {
            var apiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_redditOptions.ApplicationId}:{_redditOptions.ApplicationSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", apiKey);
            var values = new Dictionary<string, string>
            {
                { "grant_type",  _redditOptions.GrantType}
            };
            var content = new FormUrlEncodedContent(values);
            var postResponse = await _httpClient.PostAsync("access_token", content, cancellationToken);
            postResponse.EnsureSuccessStatusCode();
            var json = await postResponse.Content.ReadAsStringAsync();
            var accessToken = JsonSerializer.Deserialize<RedditToken>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

            return accessToken;
        }
    }
}

