using System.Linq;
using Discord.WebSocket;

namespace Felicity.Helpers
{
    internal static class Extensions
    {
        public static bool IsStaff(this SocketUser user)
        {
            var staff = ConfigHelper.GetBotSettings();
            return staff.BotStaff.Any(staffId => staffId == user.Id);
        }
    }
}
