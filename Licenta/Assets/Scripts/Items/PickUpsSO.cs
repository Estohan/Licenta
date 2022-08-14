using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUp Items")]
public class PickUpsSO : ScriptableObject {
    [SerializeField]
    private List<(int, GameObject)> commonItems;
    [SerializeField]
    private List<(int, GameObject)> rareItems;
    [SerializeField]
    private List<(int, GameObject)> epicItems;
    [SerializeField]
    private List<(int, GameObject)> legendaryItems;

    public GameObject GetPickUpItem(int itemID, ItemRarity rarity) {
        List<(int, GameObject)> container;

        // Select items containter of required rarity
        switch (rarity) {
            case ItemRarity.Common:
                container = commonItems;
                break;
            case ItemRarity.Rare:
                container = rareItems;
                break;
            case ItemRarity.Epic:
                container = epicItems;
                break;
            default: // legendary rarity
                container = legendaryItems;
                break;
        }

        // Search for object of id ItemID
        foreach ((int id, GameObject obj) in container) {
            if (id == itemID) {
                return obj;
            }
        }

        return null;
    }

}
