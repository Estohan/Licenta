using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      This holds the data of a PickUp game object and ensures its correct
 *  behavior when picked up by the player.
 */
[System.Serializable]
public class PickUpItem : MonoBehaviour {
    [SerializeField]
    private string itemName;
    [SerializeField]
    private ItemRarity itemRarity;
    [SerializeField]
    private int itemID;
    [SerializeField]
    private PickUpEffect itemProperties;


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            // Display notification
            InGameUI.UINotifications.instance.DisplayNotification(itemName);
            // Apply item effect
            itemProperties.ApplyPickUpEffect(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    public ItemRarity GetItemRarity() {
        return itemRarity;
    }

    public int GetItemID() {
        return itemID;
    }

    public string GetItemName() {
        return itemName;
    }
}
