using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerStats : MyMonoBehaviour {
    public GameObject healthObject;
    public GameObject positionObject;
    public PlayerStats playerStats;

    private TextMeshProUGUI healthText;
    private TextMeshProUGUI positionText;

    private float displayedHealthValue;
    //MazeCoords displayedPlayerPosition;

    private void Awake() {
        healthText = healthObject.GetComponent<TextMeshProUGUI>();
        positionText = positionObject.GetComponent<TextMeshProUGUI>();
    }

    protected override void Start() {
        base.Start();
        if (healthText != null) {
            UpdateHealth(playerStats.maxHealth);
        }

        if (positionText != null) {
            UpdatePlayerPosition(playerStats.currentCellData);
        }
    }

    private void UpdateHealth(float newValue) {
        displayedHealthValue = newValue;
        healthText.text = newValue.ToString();
    }

    private void UpdatePlayerPosition(MazeCellData newCurrentCellData) {
        //displayedPlayerPosition = newCurrentCellData.coordinates;
        CellStats newCurrentCellStats = newCurrentCellData.cellStats;
        positionText.text = "Coords.   \t:" + newCurrentCellData.coordinates.ToString();
        positionText.text += "\nInSol.  \t:" + newCurrentCellStats.isInSolution.ToString();
        positionText.text += "\nDead-end\t:" + newCurrentCellStats.isDeadEnd.ToString();
        positionText.text += "\nAdj.gate\t:" + newCurrentCellStats.isAdjacentToSectorGate.ToString();

        positionText.text += "\nDist.sol\t:" + newCurrentCellStats.distanceToSolution.ToString();
        positionText.text += "\nAcc.pt. \t:" + newCurrentCellStats.accessPoints.ToString();

        positionText.text += "\nRch.from\t:" + newCurrentCellStats.reachableFrom.ToString();
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void SafeOnEnable() {
        // events
        GameEventSystem.instance.OnPlayerStatsChange += PlayerStatsChangeReaction;
    }

    private void PlayerStatsChangeReaction(object sender, EventArgs e) {
        float damage = displayedHealthValue - playerStats.currHealth;
        if(damage > 0) {
            UpdateHealth(playerStats.currHealth);
        }
        UpdatePlayerPosition(playerStats.currentCellData);
    }

    private void OnDisable() {
        GameEventSystem.instance.OnPlayerStatsChange -= PlayerStatsChangeReaction;
    }

    /*private void HitPlayerReaction(object sender, EventArgs e) {
        Debug.Log("UI Player Stats: Player was hit!");
    }*/

    /*private void HitPlayerReaction(object sender, float damage) {
        //Debug.Log("UI Player Stats: Player was hit for " + damage + " damage!");
        StartCoroutine(__Wait(damage)); // -------------------------------------------------------------------- <<<<
        //healthText.text = playerStats.currHealth.ToString();
    }*/

    /*private IEnumerator __Wait(float damage) {
        yield return new WaitForSeconds(1);
        healthText.text = playerStats.currHealth.ToString();
        Debug.Log("UI Player Stats: Player was hit for " + damage + " damage!");
    }*/
}
