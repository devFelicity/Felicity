using Discord;
using FelicityOne.Enums;
using FelicityOne.Helpers;
using FelicityOne.Services;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace FelicityOne.Caches;

public class CheckpointCache
{
    [J("CPInventory")] public CpInventory CpInventory { get; set; } = new();
}

public class CpInventory
{
    [J("Expires")] public DateTime Expires { get; set; }
    [J("Checkpoints")] public Checkpoint[] Checkpoints { get; set; } = Array.Empty<Checkpoint>();
    [J("SavedCheckpoints")] public Checkpoint[] SavedCheckpoints { get; set; } = Array.Empty<Checkpoint>();
}

public class Checkpoint
{
    [J("name")] public string Name { get; set; } = null!;
    [J("join")] public string Join { get; set; } = null!;
}

public static class ProcessCPData
{
    private const string filePath = "Data/cpCache.json";

    public static void Populate(IMessage socketMessage)
    {
        if (!socketMessage.Content.Contains("𝐂𝐇𝐄𝐂𝐊𝐏𝐎𝐈𝐍𝐓 𝐒𝐇𝐀𝐑𝐈𝐍𝐆"))
            return;

        var messageLines = socketMessage.Content.Split(new[] {'\n'});

        var currentTime = DateTime.Now;

        var newCache = new CheckpointCache
        {
            CpInventory =
            {
                Expires = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day + 1, 6, 0, 0)
            }
        };

        var savedCpStart = 0;
        var savedCpEnd = 0;

        var activeCpStart = 0;
        var activeCpEnd = messageLines.Length;

        for (var i = 0; i < messageLines.Length; i++)
        {
            if (messageLines[i].ToLower().Contains("saved checkpoints") && savedCpStart == 0)
                savedCpStart = i + 1;

            if (messageLines[i].ToLower().StartsWith("*for saved checkpoints,"))
                savedCpEnd = i;

            // ReSharper disable once InvertIf
            if (messageLines[i].ToLower().Contains("active checkpoints and join codes"))
            {
                activeCpStart = i + 1;
                break;
            }
        }

        var savedCpList = new List<Checkpoint>();
        var activeCpList = new List<Checkpoint>();

        for (var i = savedCpStart; i < savedCpEnd; i++)
            savedCpList.Add(new Checkpoint
            {
                Name = messageLines[i].Split("> ")[1].Replace("**", ""),
                Join = null!
            });

        for (var i = activeCpStart; i < activeCpEnd - 1; i++)
        {
            if (messageLines[i].Contains("/join") || string.IsNullOrEmpty(messageLines[i]))
                continue;

            activeCpList.Add(new Checkpoint
            {
                Name = messageLines[i].Split("**")[1].Split("**")[0].Replace("**", ""),
                Join = messageLines[i + 1].Split('`')[1].Split('`')[0]
            });
        }

        newCache.CpInventory.SavedCheckpoints = savedCpList.ToArray();
        newCache.CpInventory.Checkpoints = activeCpList.ToArray();

        File.WriteAllText(filePath, ConfigHelper.ToJson(newCache));

        (socketMessage as IUserMessage).ReplyAsync("Checkpoint list saved to file.");
    }

    public static Embed BuildCPEmbed(string activity)
    {
        var cpCache = ConfigHelper.FromJson<CheckpointCache>(File.ReadAllText(filePath));
        var embed = BaseEmbed();

        var activeCp =
            cpCache?.CpInventory.Checkpoints.FirstOrDefault(activeCheckpoint => activeCheckpoint.Name == activity);
        if (activeCp == null)
        {
            embed.Description = "An error occured while fetching checkpoint.";
            return embed.Build();
        }

        if (cpCache != null && cpCache.CpInventory.Expires < DateTime.Now)
            embed.Description += "⚠️ **WARNING: THESE CHECKPOINTS MAY BE OUT OF DATE** ⚠️\n\n";

        embed.AddField(activeCp.Name, activeCp.Join);

        return embed.Build();
    }

    public static Embed BuildSavedEmbed()
    {
        var cpCache = ConfigHelper.FromJson<CheckpointCache>(File.ReadAllText(filePath));

        var savedCpList = cpCache?.CpInventory.SavedCheckpoints.Select(savedCheckpoint => savedCheckpoint.Name)
            .ToList();

        var embed = BaseEmbed();
        embed.Title = "Saved checkpoints:";

        if (savedCpList != null)
            foreach (var savedCp in savedCpList)
                embed.Description += $"{savedCp}\n";

        embed.Description +=
            "\nTo use these checkpoints, join the [Checkpoint Server](https://discord.gg/Mecu7KvjuS), " +
            "go to the [#checkpoints-chat](https://discord.com/channels/900591826975752192/911705214116053094), " +
            "and tag the Checkpoint Helper role with your request.";

        return embed.Build();
    }

    public static Embed BuildServerEmbed()
    {
        var embed = BaseEmbed();
        embed.Description =
            "For help, saved checkpoints and nice people to talk to, join the **Luckstruck9** Discord Server by clicking [here](https://discord.gg/Mecu7KvjuS).\n\n" +
            "**DO NOT** post Felicity errors in this server, use the [Felicity support server](https://discord.gg/JBBqF6Pw2z) for that.";

        return embed.Build();
    }

    private static EmbedBuilder BaseEmbed()
    {
        return new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = "Luckstruck9",
                IconUrl = "https://cdn.discordapp.com/icons/900591826975752192/95149975a7163234773ff26f73bd43e2.png",
                Url = "https://twitter.com/Luckstruck9"
            },
            Color = ConfigService.GetEmbedColor(),
            Footer = new EmbedFooterBuilder
            {
                Text = Strings.FelicityVersion,
                IconUrl = Images.FelicityLogo
            }
        };
    }
}