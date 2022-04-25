using System;
using Newtonsoft.Json;

namespace Felicity.Enums;

using J = JsonPropertyAttribute;

public partial class TiltifyCampaign
{
    [J("meta")] public Meta Meta { get; set; }
    [J("data")] public Data Data { get; set; }
}

public class Data
{
    [J("id")] public long Id { get; set; }
    [J("name")] public string Name { get; set; }
    [J("slug")] public string Slug { get; set; }
    [J("startsAt")] public long StartsAt { get; set; }
    [J("endsAt")] public long? EndsAt { get; set; }
    [J("description")] public string Description { get; set; }
    [J("causeId")] public long CauseId { get; set; }
    [J("originalFundraiserGoal")] public double OriginalFundraiserGoal { get; set; }
    [J("fundraiserGoalAmount")] public double FundraiserGoalAmount { get; set; }
    [J("supportingAmountRaised")] public double SupportingAmountRaised { get; set; }
    [J("amountRaised")] public double AmountRaised { get; set; }
    [J("supportable")] public bool Supportable { get; set; }
    [J("status")] public string Status { get; set; }
    [J("type")] public string Type { get; set; }
    [J("avatar")] public Avatar Avatar { get; set; }
    [J("livestream")] public Livestream Livestream { get; set; }
    [J("causeCurrency")] public string CauseCurrency { get; set; }
    [J("totalAmountRaised")] public double TotalAmountRaised { get; set; }
    [J("user")] public User User { get; set; }
    [J("regionId")] public int? RegionId { get; set; }
    [J("metadata")] public Metadata Metadata { get; set; }
}

public class Avatar
{
    [J("src")] public Uri Src { get; set; }
    [J("alt")] public string Alt { get; set; }
    [J("width")] public int Width { get; set; }
    [J("height")] public int Height { get; set; }
}

public class Livestream
{
    [J("type")] public string Type { get; set; }
    [J("channel")] public string Channel { get; set; }
}

public class Metadata
{
}

public class User
{
    [J("id")] public long Id { get; set; }
    [J("username")] public string Username { get; set; }
    [J("slug")] public string Slug { get; set; }
    [J("url")] public Uri Url { get; set; }
    [J("avatar")] public Avatar Avatar { get; set; }
}

public class Meta
{
    [J("status")] public long Status { get; set; }
}