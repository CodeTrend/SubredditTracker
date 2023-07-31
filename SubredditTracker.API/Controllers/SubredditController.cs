using System.Net;
using Microsoft.AspNetCore.Mvc;
using SubredditTracker.API.Interfaces;
using SubredditTracker.Domain.Interfaces;

namespace SubredditTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubredditController : ControllerBase
{
    private readonly ILogger<SubredditController> _logger;
    private readonly IRedditDataService _redditDataService;
    private readonly ICachingService _cache;

    public SubredditController(ICachingService cache, ILogger<SubredditController> logger, IRedditDataService redditDataService)
    {
        _logger = logger;
        _redditDataService = redditDataService;
        _cache = cache;
    }

    [HttpGet("Subscribe/{subreddit}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public ActionResult<string> Subscribe(string subreddit)
    {
        var validUntil = DateTime.UtcNow.AddDays(1);
        _cache.Set(SubredditTracker.API.Utils.CacheConstants.SubRedditKey, validUntil, subreddit);
        return Ok("Subscribed");
    }

    [HttpGet("Unsubscribe/{subreddit}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public ActionResult<string> Unsubscribe(string subreddit)
    {
        var validUntil = DateTime.UtcNow.AddDays(1);
        _cache.Remove(SubredditTracker.API.Utils.CacheConstants.SubRedditKey);
        return Ok("Unsubscribed");
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public ActionResult<string> GetSubscription()
    {
        if (_cache.TryGetValue(SubredditTracker.API.Utils.CacheConstants.SubRedditKey, out string subreddit))
        {
            return Ok(subreddit);
        }
        return NotFound();

    }
}

