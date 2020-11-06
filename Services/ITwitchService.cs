using System.Collections.Generic;
using System.Threading.Tasks;
using snipetrain_bot.Models;

namespace snipetrain_bot.Services
{
    public interface ITwitchService
    {
        Task<TwitchUser> GetTwitchUser(string twitchUser);
        Task AuthenticateTwitch();
        void AddTwitchSubscription(string twitchUsername, string twitchUserId);
        void RemoveTwitchSubscription(string twitchUsername);
        Task AddAllTwitchSubscriptions();
    }
}