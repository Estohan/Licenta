using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickUpItem : MonoBehaviour {
    [SerializeField]
    private ItemRarity rarity;
    [SerializeField]
    private int itemID;

    /*[Space]
    [Header("Item specific fields:")]*/

    private void OnTriggerEnter(Collider other) {
        this.enabled = false;
        ItemEffect();
    }

    public ItemRarity GetItemRarity() {
        return rarity;
    }

    public int GetItemID() {
        return itemID;
    }

    public virtual void ItemEffect() {

    }
}
