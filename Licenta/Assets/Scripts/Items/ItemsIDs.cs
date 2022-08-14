using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemsIDs {
    public static readonly List<(int, int)> commonItemsIDs = new List<(int, int)> {
        (1, 50), // 5 HP heal
        (2, 100) // 10 HP heal
    };

    public static readonly List<(int, int)> rareItemsIDs = new List<(int, int)> {
        (1, 60), // 15 HP heal
        (2, 100) // 20 HP heal
    };

    public static readonly List<(int, int)> epicItemsIDs = new List<(int, int)> {
        (1, 70), // 10% HP heal
        (2, 100) // 15% HP heal
    };

    public static readonly List<(int, int)> legendaryItemsIDs = new List<(int, int)> {
        (1, 20), // health upgrade
        (2, 40), // map reveal
        (3, 70), // beacon
        (4, 100) // full heal
    };
}

public enum ItemRarity {
    Common,
    Rare,
    Epic,
    Legendary
}
