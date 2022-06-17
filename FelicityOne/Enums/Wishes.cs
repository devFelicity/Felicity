namespace FelicityOne.Enums;

internal static class Wishes
{
    public static readonly List<Wish> KnownWishes = new()
    {
        new Wish {Name = "A wish to feed an addiction.", Description = "grants an Ethereal Key", Number = 1},
        new Wish {Name = "A wish for material validation.", Description = "spawns Glittering Key chest", Number = 2},
        new Wish {Name = "A wish for others to celebrate your success.", Description = "unlocks Numbers of Power emblem", Number = 3},
        new Wish {Name = "A wish to look athletic and elegant.", Description = "teleport to Shuro-Chi", Number = 4},
        new Wish {Name = "A wish for a promising future.", Description = "teleport to Morgeth", Number = 5},
        new Wish {Name = "A wish to move the hands of time.", Description = "teleport to Vault", Number = 6},
        new Wish {Name = "A wish to help a friend in need.", Description = "teleport to Riven", Number = 7},
        new Wish {Name = "A wish to stay here forever.", Description = "plays Hope for the Future song", Number = 8},
        new Wish {Name = "A wish to stay here forever.", Description = "enables Failsafe voice lines", Number = 9},
        new Wish {Name = "A wish to stay here forever.", Description = "enables Drifter voice lines", Number = 10},
        new Wish {Name = "A wish to stay here forever.", Description = "enables Grunt Birthday Party", Number = 11},
        new Wish {Name = "A wish to open your mind to new ideas.", Description = "adds an effect around the players head", Number = 12},
        new Wish {Name = "A wish for the means to feed an addiction.", Description = "enables Petra's Run (flawless)", Number = 13},
        new Wish {Name = "A wish for love and support.", Description = "spawns Corrupted Eggs", Number = 14},
        new Wish {Name = "This one you shall cherish.", Description = "undiscovered", Number = 15}
    };
}

internal class Wish
{
    public string Name { get; init; }
    public string Description { get; init; }
    public int Number { get; init; }
}