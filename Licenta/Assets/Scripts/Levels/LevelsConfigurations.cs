using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelsConfigurations")]
public class LevelsConfigurations : ScriptableObject {

    public List<LevelConfiguration> levelsConfigs;
    
}

[System.Serializable]
public class LevelConfiguration {
    public int sizeZ;
    public int sizeX;
    public int outerPaddingPerc;
    public int innerPaddingPerc;
    public int nrOfSectors;

    public int stage;

    public int difficulty;
    [Tooltip("Chance of obstacle downgrade.")]
    public int chanceOfObstDowngrade;
    [Tooltip("Chance of obstacle upgrade.")]
    public int chanceOfObstUpgrade;

    [Tooltip("Chance of a room being empty.")]
    public float noItemDropChance;
    [Header("This MUST have four rarity drop chances!")]
    public List<RarityDropChance> itemDropChances;

    /*public LevelConfiguration(int sizeZ,
                                int sizeX,
                                int outerPaddingPerc,
                                int innerPaddingPerc,
                                int nrOfSectors) {
        this.sizeZ = sizeZ;
        this.sizeX = sizeX;
        this.outerPaddingPerc = outerPaddingPerc;
        this.innerPaddingPerc = innerPaddingPerc;
        this.nrOfSectors = nrOfSectors;
        this.stage = 0;
        this.difficulty = 0;
        this.chanceOfObstDowngrade = 20;
        this.chanceOfObstUpgrade = 20;
        this.noItemDropChance = 30f;
        itemDropChances = new List<RarityDropChancePair> {
                new RarityDropChancePair(ItemRarity.Common, 65f), // 50% chance of common drop
                new RarityDropChancePair(ItemRarity.Rare, 85f), // 30% chance of rare drop
                new RarityDropChancePair(ItemRarity.Epic, 99f), // 19% chance of epic drop
                new RarityDropChancePair(ItemRarity.Legendary, 100f) // 1% chance of legendary drop
            };
    }*/
}

[System.Serializable]
public class RarityDropChance {
    public ItemRarity rarity;
    [Tooltip("real_CommonDropChance = this_CommondropChance\n" +
            "real_RareDropChance = this_RareDropChance - this_CommondropChance\n" +
            "real_EpicDropChance = this_EpicDropChance - this_RaredropChance\n" +
            "real_LegendaryDropChance = this_LegendaryDropChance - this_EpicdropChance")]
    public float dropChance;

    /*public RarityDropChancePair(ItemRarity rarity, float dropChance) {
        this.rarity = rarity;
        this.dropChance = dropChance;
    }*/
}
