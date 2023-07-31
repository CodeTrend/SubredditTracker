using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SubredditTracker.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public async Task OnPostAsync()
    {
        var subredditToTrack = Request.Form["subredditToTrack"];
    }
}

