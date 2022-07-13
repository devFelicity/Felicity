using System.Text.Json;
using Discord;
using Felicity.Util;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace Felicity.Models.Caches;

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

public static class ProcessCpData
{
    private const string FilePath = "Data/cpCache.json";

    public static CheckpointCache? ReadJson()
    {
        return JsonSerializer.Deserialize<CheckpointCache>(File.ReadAllText(FilePath));
    }

    private static void WriteJson(this CheckpointCache cpCache)
    {
        File.WriteAllText(FilePath,
            JsonSerializer.Serialize(cpCache, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static void Populate(IMessage socketMessage)
    {
        if (socketMessage.Embeds.Count != 1)
            return;

        if (!socketMessage.Embeds.First().Title.ToLower().Contains("checkpoint sharing"))
            return;

        var embed = socketMessage.Embeds.ElementAt(0);
        var generalInfoField = embed.Fields[0];
        var savedCpField = embed.Fields[3];

        var offlineTime =
            MiscUtils.TimeStampToDateTime(Convert.ToDouble(generalInfoField.Value.Split("<t:")[1].Split(":R>")[0]));

        var newCache = new CheckpointCache
        {
            CpInventory =
            {
                Expires = offlineTime
            }
        };

        var activeCpList = new List<Checkpoint>();

        for (var i = 5; i < embed.Fields.Length; i++)
        {
            activeCpList.Add(new Checkpoint
            {
                Name = embed.Fields[i].Name.Split(">")[1].Replace("**", "").TrimEnd(' ', '\t'),
                Join = embed.Fields[i].Value.Split("`")[1].Replace("`", "").Replace("  ", " ")
            });
        }

        newCache.CpInventory.SavedCheckpoints = savedCpField.Value.Split("\n").Select(line =>
            new Checkpoint { Name = line.Split(">")[1].Replace("**", "").TrimEnd(' ', '\t') }).ToArray();
        newCache.CpInventory.Checkpoints = activeCpList.ToArray();

        newCache.WriteJson();

        (socketMessage as IUserMessage).ReplyAsync("Checkpoint list saved to file.");
    }

    public static Embed BuildCpEmbed(string activity)
    {
        var cpCache = ReadJson();
        var embed = BaseEmbed();

        var activeCp =
            cpCache?.CpInventory.Checkpoints.FirstOrDefault(activeCheckpoint => activeCheckpoint.Name == activity);
        if (activeCp == null)
        {
            embed.Description = "An error occurred while fetching checkpoint.";
            return embed.Build();
        }

        if (cpCache != null && cpCache.CpInventory.Expires < DateTime.Now)
            embed.Description += "⚠️ **WARNING: CHECKPOINT MAY BE OUT OF DATE** ⚠️\n\n";

        embed.AddField(activeCp.Name, activeCp.Join);

        return embed.Build();
    }

    public static Embed BuildSavedEmbed()
    {
        var cpCache = JsonSerializer.Deserialize<CheckpointCache>(File.ReadAllText(FilePath));

        var savedCpList = cpCache?.CpInventory.SavedCheckpoints.Select(savedCheckpoint => savedCheckpoint.Name)
            .ToList();

        var embed = BaseEmbed();
        embed.Title = "Saved checkpoints:";

        if (savedCpList != null)
            foreach (var savedCp in savedCpList)
                embed.Description += $"{savedCp}\n";

        embed.Description +=
            "\nTo use these checkpoints, join the [Checkpoint Server](https://discord.gg/luckstruck9), " +
            "go to the [#checkpoints-chat](https://discord.com/channels/900591826975752192/911705214116053094), " +
            "and use `/checkpointstatus` with **Luck Bot** there.";

        return embed.Build();
    }

    public static Embed BuildServerEmbed()
    {
        var embed = BaseEmbed();
        embed.Description =
            "For help, saved checkpoints and nice people to talk to, join the **Luckstruck9** Discord Server by clicking [here](https://discord.gg/luckstruck9).\n\n" +
            $"**DO NOT** post Felicity errors in this server, use the [Felicity support server]({BotVariables.DiscordInvite}) for that.";

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
                Url = "https://discord.gg/Luckstruck9"
            },
            Color = Color.Blue,
            Footer = Embeds.MakeFooter()
        };
    }
}