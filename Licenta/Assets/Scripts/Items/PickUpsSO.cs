using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Scriptable object that is used by the ObjectDatabase and contains
 *  references to all PickUp game objects, sorted by their rarity types.
 */
[CreateAssetMenu(menuName = "PickUp Items")]
public class PickUpsSO : ScriptableObject {
    [SerializeField]
    private List<PickUpItem> commonItems;
    [SerializeField]
    private List<PickUpItem> rareItems;
    [SerializeField]
    private List<PickUpItem> epicItems;
    [SerializeField]
    private List<PickUpItem> legendaryItems;

    public GameObject GetPickUpItem(int itemID, ItemRarity rarity) {
        List<PickUpItem> container;

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
        foreach (PickUpItem item in container) {
            if (item.GetItemID() == itemID) {
                return item.gameObject;
            }
        }

        return null;
    }
}
