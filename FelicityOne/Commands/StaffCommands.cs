using BungieSharper.Entities.Destiny.Definitions;
using BungieSharper.Entities.Destiny.Definitions.ActivityModifiers;
using Discord;
using Discord.Commands;
using FelicityOne.Caches;
using FelicityOne.Enums;
using FelicityOne.Helpers;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace FelicityOne.Commands;

public class StaffCommands : ModuleBase<SocketCommandContext>
{
    [Command("listServers")]
    public async Task List()
    {
        var serverList = Context.Client.Guilds.OrderBy(x => x.Name);
        var msg = string.Empty;
        var i = 0;
        foreach (var guild in serverList)
        {
            if (i == 10)
            {
                await ReplyAsync(msg);
                msg = "";
                i = 0;
            }

            msg += $"- {Format.Bold(guild.Name)} ({guild.Id}) [{Format.Italics(guild.Owner.ToString())}]\n";
            i++;
        }

        await ReplyAsync(msg);
    }

    [Command("fillCPs")]
    public Task FillCPs(ulong messageId)
    {
        var msg = Context.Channel.GetMessageAsync(messageId).Result;
        ProcessCPData.Populate(msg);
        return Task.CompletedTask;
    }

    [Command("nf")]
    public async Task Nightfall(int difficulty)
    {
        if (!Enumerable.Range(0, 4).Contains(difficulty))
        {
            await ReplyAsync("Difficulty out of range (0-4)");
            return;
        }

        var milestones = BungieAPI.GetApiClient().Api.Destiny2_GetPublicMilestones().Result;

        var nightfall = milestones[1942283261];
        var gm = nightfall.Activities.ElementAt(difficulty);

        var activity = BungieAPI.GetManifestDefinition<DestinyActivityDefinition>(Lang.En, new[] {gm.ActivityHash})
            .First();

        var embed = new EmbedBuilder
        {
            Title = activity.DisplayProperties.Description,
            Description = activity.DisplayProperties.Name,
            ThumbnailUrl = BungieAPI.BaseUrl + activity.DisplayProperties.Icon,
            Footer = Extensions.GenerateEmbedFooter()
        };

        var modifiers = BungieAPI.GetManifestDefinition<DestinyActivityModifierDefinition>(Lang.En, gm.ModifierHashes);

        /* Champion Foes
         *
         * 40182179
         * 197794292https://www.youtube.com/
         * 438106166
         * 1598783516
         * 1806568190
         * 1990363418
         * 2006149364
         * 2475764450
         * 3307318061
         * 4038464106
         *
         * Diff Modifiers
         *
         * 518117495 (gm)
         * 3788294071 (master)
         * 2116552995 (master)
         * 4123192267 (legend)
         * 2421815503 (legend)
         * 2060034565 (hero)
         *
         * Double Vanguard Rank
         *
         * 745014575
         *
         * Shielded Foes
         *
         * 93790318
         * 720259466
         * 1270996828
         * 1377274412
         * 2288210988
         * 2650740350
         * 2965677044
         * 3119632620
         * 3171609188
         */

        var activeModifiers = modifiers.Where(destinyActivityModifierDefinition =>
            !string.IsNullOrEmpty(destinyActivityModifierDefinition.DisplayProperties.Name) &&
            destinyActivityModifierDefinition.DisplayInNavMode).ToList();

        var modifierList = activeModifiers.Aggregate("",
            (current, modifier) =>
                current +
                $"{Format.Bold(modifier.DisplayProperties.Name)} (id: {modifier.Hash})\n{Format.Italics(modifier.DisplayProperties.Description)}\n\n");

        embed.AddField("Modifiers", modifierList, true);

        await ReplyAsync(embed: embed.Build());
    }

    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public InteractiveService Interactive { get; set; }

    [Command("pageTest", RunMode = RunMode.Async)]
    public async Task PaginatorAsync()
    { 
        IPageBuilder[] pages = {
            new PageBuilder().WithDescription("Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
            new PageBuilder().WithDescription("Praesent eu est vitae dui sollicitudin volutpat."),
            new PageBuilder().WithDescription("Etiam in ex sed turpis imperdiet viverra id eget nunc."),
            new PageBuilder().WithDescription("Donec eget feugiat nisi. Praesent faucibus malesuada nulla, a vulputate velit eleifend ut.")
        };

        var stopEmote = EmoteHelper.GetEmote("", "felicity", 0) ?? (IEmote?) new Emoji("🛑");

#pragma warning disable CS8604 // Possible null reference argument.
        var paginator = new StaticPaginatorBuilder()
            .AddUser(Context.User)
            .WithPages(pages)
            .WithActionOnCancellation(ActionOnStop.DeleteInput)
            .WithActionOnTimeout(ActionOnStop.DisableInput)
            .AddOption(new Emoji("◀"), PaginatorAction.Backward)
            .AddOption(new Emoji("▶"), PaginatorAction.Forward)
            .AddOption(new Emoji("🔢"), PaginatorAction.Jump)
            .AddOption(stopEmote, PaginatorAction.Exit)
            .Build();
#pragma warning restore CS8604 // Possible null reference argument.

        await Interactive.SendPaginatorAsync(paginator, Context.Channel);
    }
}