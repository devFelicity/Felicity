namespace FelicityOne.Enums;

internal static class Wishes
{
    public static readonly List<Wish> KnownWishes = new()
    {
        new Wish {Name = "A wish to feed an addiction.", Description = "Grants an Ethereal Key", Number = 1},
        new Wish {Name = "A wish for material validation.", Description = "Spawns Glittering Key chest", Number = 2},
        new Wish {Name = "A wish for others to celebrate your success.", Description = "Unlocks Numbers of Power emblem", Number = 3},
        new Wish {Name = "A wish to look athletic and elegant.", Description = "Teleport to Shuro-Chi", Number = 4},
        new Wish {Name = "A wish for a promising future.", Description = "Teleport to Morgeth", Number = 5},
        new Wish {Name = "A wish to move the hands of time.", Description = "Teleport to Vault", Number = 6},
        new Wish {Name = "A wish to help a friend in need.", Description = "Teleport to Riven", Number = 7},
        new Wish {Name = "A wish to stay here forever.", Description = "Plays Hope for the Future song", Number = 8},
        new Wish {Name = "A wish to stay here forever.", Description = "Enables Failsafe voice lines", Number = 9},
        new Wish {Name = "A wish to stay here forever.", Description = "Enables Drifter voice lines", Number = 10},
        new Wish {Name = "A wish to stay here forever.", Description = "Enables Grunt Birthday Party", Number = 11},
        new Wish {Name = "A wish to open your mind to new ideas.", Description = "Adds an effect around the players head", Number = 12},
        new Wish {Name = "A wish for the means to feed an addiction.", Description = "Enables Petra's Run (flawless)", Number = 13},
        new Wish {Name = "A wish for love and support.", Description = "Spawns Corrupted Eggs", Number = 14},
        new Wish {Name = "This one you shall cherish.", Description = "Undiscovered", Number = 15}
    };
}

internal class Wish
{
    public string Name { get; init; }
    public string Description { get; init; }
    public int Number { get; init; }
}