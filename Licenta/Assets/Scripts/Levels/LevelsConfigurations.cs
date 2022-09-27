using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Scriptable object that stores the individual configurations of every
 *  game level. These are used when a new level layout is generated.
 */
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
}

[System.Serializable]
public class RarityDropChance {
    public ItemRarity rarity;
    [Tooltip("real_CommonDropChance = this_CommondropChance\n" +
            "real_RareDropChance = this_RareDropChance - this_CommondropChance\n" +
            "real_EpicDropChance = this_EpicDropChance - this_RaredropChance\n" +
            "real_LegendaryDropChance = this_LegendaryDropChance - this_EpicdropChance")]
    public float dropChance;
}
