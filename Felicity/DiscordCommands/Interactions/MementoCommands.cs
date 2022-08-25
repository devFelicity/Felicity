using Discord;
using Discord.Interactions;
using Felicity.Models.Caches;
using Felicity.Util;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Felicity.DiscordCommands.Interactions;

public class MementoCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("memento", "Curious to see how a memento will look?")]
    public async Task Memento(
        [Summary("source", "Where does the weapon you want to check out come from?")]
        MementoSource memSource,
        [Autocomplete(typeof(MementoWeaponAutocomplete))] [Summary("weapon", "Name of the weapon you want to see.")]
        string memWeapon,
        [Summary("type", "What type of memento are you looking for?")]
        MementoType memType
    )
    {
        await DeferAsync();

        var memCache = ProcessMementoData.ReadJson();
        if (memCache == null)
        {
            await FollowupAsync("Error fetching memento cache.");
            return;
        }

        MementoCache.MementoWeaponList goodWeapon = null!;

        foreach (var mementoInventoryElement in memCache.MementoInventory!)
        foreach (var weapon in mementoInventoryElement.WeaponList!)
            if (weapon.WeaponName == memWeapon)
                goodWeapon = weapon;

        if (goodWeapon == null!)
        {
            await FollowupAsync("An error occurred while fetching memento, try filling arguments in order.");
            return;
        }
        
        var goodMemento = goodWeapon.TypeList?.FirstOrDefault(x => x.Type == memType)?.Memento;

        if (goodMemento?.Credit == "NoImage")
        {
            var errorEmbed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "TheLastJoaquin",
                    IconUrl = BotVariables.Images.JoaquinAvatar,
                    Url = "https://twitter.com/TheLastJoaquin"
                },
                Color = Color.Red,
                Description =
                    $"Sorry! We don't currently have the Memento image for **{goodWeapon.WeaponName}** ({memType}) :(\n\n" +
                    $"If you have it and would like to submit it, please head to our [Support Server]({BotVariables.DiscordInvite}) and send it to us there!"
            };

            await FollowupAsync(embed: errorEmbed.Build());
            return;
        }

        var embedColor = memType switch
        {
            MementoType.Gambit => Color.Green,
            MementoType.Nightfall => Color.Orange,
            MementoType.Trials => Color.Gold,
            _ => Color.Blue
        };

        var embed = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = "TheLastJoaquin",
                IconUrl = BotVariables.Images.JoaquinAvatar,
                Url = "https://twitter.com/TheLastJoaquin"
            },
            Title = "Memento Preview:",
            Description =
                $"This is what **{goodWeapon.WeaponName}** looks like with a **{memType}** Memento equipped.",
            Color = embedColor,
            Fields = new List<EmbedFieldBuilder>
            {
                new() { IsInline = true, Name = "Source", Value = GetMementoSourceString(memSource) },
                new() { IsInline = true, Name = "Credit", Value = GetCredit(goodMemento?.Credit) }
            },
            ImageUrl = goodMemento?.ImageUrl,
            Footer = Embeds.MakeFooter(),
            ThumbnailUrl = GetMementoImage(memType)
        };

        await FollowupAsync(embed: embed.Build());
    }

    private static string GetCredit(string? goodMementoCredit)
    {
        if (goodMementoCredit is null or "Unknown")
            return "Unknown";

        if (goodMementoCredit.StartsWith("/u/"))
            return $"[{goodMementoCredit}](https://reddit.com{goodMementoCredit})";

        if (goodMementoCredit.StartsWith("@"))
            return $"[{goodMementoCredit}](https://twitter.com/{goodMementoCredit})";

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (goodMementoCredit.Equals("yt/Benny Clips"))
            return $"[{goodMementoCredit}](https://www.youtube.com/channel/UCfxAFtfQnN0fpUY6WD_Z5wQ)";

        return goodMementoCredit;
    }

    private static string GetMementoImage(MementoType memType)
    {
        return memType switch
        {
            MementoType.Gambit =>
                "https://bungie.net/common/destiny2_content/icons/045e66a538f70024c194b01a5cf8652a.jpg",
            MementoType.Trials =>
                "https://bungie.net/common/destiny2_content/icons/c2e0148851bd8aec5d04d413b897dcbd.jpg",
            MementoType.Nightfall =>
                "https://bungie.net/common/destiny2_content/icons/bf21c13f03a29aa0067f85c84593a594.jpg",
            _ => ""
        };
    }

    private static string GetMementoSourceString(MementoSource memSource)
    {
        var goodSource = memSource switch
        {
            MementoSource.OpenWorld => "Open World",
            MementoSource.RaidVotD => "Vow of the Disciple",
            MementoSource.SeasonRisen => "Seasonal (Risen)",
            MementoSource.SeasonHaunted => "Seasonal (Haunted)",
            MementoSource.ThroneWorld => "Throne World",
            _ => "Unknown"
        };

        return goodSource;
    }
}