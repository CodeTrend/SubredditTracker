using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using SubredditTracker.API.Interfaces;
using SubredditTracker.API.Services;
using SubredditTracker.Domain;
//using SubredditTracker.API.Hubs;
//using SubredditTracker.API.Interfaces;
//using SubredditTracker.API.Models;
using SubredditTracker.Domain.Interfaces;

namespace SubredditTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubredditController : ControllerBase
{
    //private readonly IHubContext<ChartHub> _hub;
    //private readonly TimerManager _timer;

    private readonly ILogger<SubredditController> _logger;
    //private IHubContext<TrackingHub, ITrackingHubClient> _hub;
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
        //TODO: subreddit input validation
        var validUntil = DateTime.UtcNow.AddDays(1);
        _cache.Set(SubredditTracker.API.Utils.CacheConstants.SubRedditKey, validUntil, subreddit);
        return Ok("Subscribed");
    }

    [HttpGet("Unsubscribe/{subreddit}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public ActionResult<string> Unsubscribe(string subreddit)
    {
        //TODO: subreddit input validation
        var validUntil = DateTime.UtcNow.AddDays(1);
        _cache.Remove(SubredditTracker.API.Utils.CacheConstants.SubRedditKey);
        return Ok("Unsubscribed");
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    public ActionResult<string> GetSubription()
    {
        if (_cache.TryGetValue(SubredditTracker.API.Utils.CacheConstants.SubRedditKey, out string subreddit))
        {
            return Ok(subreddit);
        }
        return NotFound();

    }
}

