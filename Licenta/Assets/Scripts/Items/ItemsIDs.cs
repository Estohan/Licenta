using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  Item ID's and their chances of getting picked.
 */
public static class ItemsIDs {

    // Lists of tuples (itemID, chanceThreshold_of_being_picked)

    public static readonly List<(int, float)> commonItemsIDs = new List<(int, float)> {
        (1, 50f), // 5 HP heal, 50% chance
        (2, 100f) // 10 HP heal, 50% chance
    };

    public static readonly List<(int, float)> rareItemsIDs = new List<(int, float)> {
        (1, 60f), // 15 HP heal, 60% chance
        (2, 100f) // 20 HP heal, 40% chance
    };

    public static readonly List<(int, float)> epicItemsIDs = new List<(int, float)> {
        (1, 70f), // 10% HP heal, 70% chance
        (2, 100f) // 15% HP heal, 30% chance
    };

    public static readonly List<(int, float)> legendaryItemsIDs = new List<(int, float)> {
        (1, 20f), // health upgrade, 20% chance
        (2, 40f), // map reveal, 20% chance
        (3, 70f), // beacon, 30% chance
        (4, 100f) // full heal, 30% chance
    };

    public static List<(int, float)> GetItemsByRarity(ItemRarity rarity) {
        switch(rarity) {
            case ItemRarity.Common:
                return commonItemsIDs;
            case ItemRarity.Rare:
                return rareItemsIDs;
            case ItemRarity.Epic:
                return epicItemsIDs;
            default:
                return legendaryItemsIDs;
        }
    }
}

public enum ItemRarity {
    Common,
    Rare,
    Epic,
    Legendary
}
