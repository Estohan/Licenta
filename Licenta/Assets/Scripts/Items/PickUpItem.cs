using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // this.enabled = false;
        if (other.CompareTag("Player")) {
            Destroy(this.gameObject);
            itemProperties.ApplyPickUpEffect(other.gameObject);
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
