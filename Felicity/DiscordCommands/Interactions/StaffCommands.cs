using Discord.Interactions;
using Felicity.Util;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Felicity.DiscordCommands.Interactions;

[Preconditions.RequireBotModerator]
public class StaffCommands : InteractionModuleBase<ShardedInteractionContext>
{ }