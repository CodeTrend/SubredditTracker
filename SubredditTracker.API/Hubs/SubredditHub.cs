
namespace SubredditTracker.API.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using SubredditTracker.Domain.Interfaces;
    using Interfaces;

    public class TrackingHub : Hub<ITrackingHubClient>
    {

        public async Task SendPostInfo(IEnumerable<ITopPost> message)
        {
            await Clients.All.PostsReceived(message);
        }

        public async Task SendUserInfo(IEnumerable<ITopUser> message)
        {
            await Clients.All.UsersReceived(message);
        }
        public async Task Error(string message)
        {
            await Clients.All.Error(message);
        }
    }
}

