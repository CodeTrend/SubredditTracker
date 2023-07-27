
namespace SubredditTracker.API.Hubs
{
    using System;
    using Microsoft.AspNetCore.SignalR;
    using SubredditTracker.Domain.Interfaces;
    using SubredditTracker.API.Interfaces;

    public class TrackingHub : Hub<ITrackingHubClient>
    {
		public TrackingHub()
		{
        }

        public async Task SendPostInfo(IEnumerable<ITopPost> message)
        {
            await Clients.All.PostsReceived(message);
        }

        public async Task SendUserInfo(IEnumerable<ITopUser> message)
        {
            await Clients.All.UsersReceived(message);
        }
    }
}

