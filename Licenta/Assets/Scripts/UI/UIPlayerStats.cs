using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerStats : MyMonoBehaviour {

    [SerializeField]
    private UIHealthBar healthBar;

    public PlayerStats playerStats;

    public GameObject positionObject;

    private TextMeshProUGUI positionText;

    //MazeCoords displayedPlayerPosition;

    private void Awake() {
        positionText = positionObject.GetComponent<TextMeshProUGUI>();
    }

    protected override void Start() {
        base.Start();

        if (positionText != null) {
            UpdatePlayerPosition(playerStats.currentCellData);
        }
    }


    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void SafeOnEnable() {
        // events
        GameEventSystem.instance.OnPlayerStatsChange += PlayerStatsChangeReaction;
    }
    private void UpdatePlayerPosition(MazeCellData newCurrentCellData) {
        //displayedPlayerPosition = newCurrentCellData.coordinates;

        /*CellStats newCurrentCellStats = newCurrentCellData.cellStats;
        positionText.text = "Coords.   \t:" + newCurrentCellData.coordinates.ToString();
        positionText.text += "\nInSol.  \t:" + newCurrentCellStats.isInSolution.ToString();
        positionText.text += "\nDead-end\t:" + newCurrentCellStats.isDeadEnd.ToString();
        positionText.text += "\nAdj.gate\t:" + newCurrentCellStats.isAdjacentToSectorGate.ToString();

        positionText.text += "\nDist.sol\t:" + newCurrentCellStats.distanceToSolution.ToString();
        positionText.text += "\nAcc.pt. \t:" + newCurrentCellStats.accessPoints.ToString();

        positionText.text += "\nRch.from\t:" + newCurrentCellStats.reachableFrom.ToString();

        positionText.text += "\nObst.\t: ";
        foreach ((int shapeID, MazeDirection direction) in newCurrentCellStats.mappedObstacleShapes) {
            positionText.text += "(" + shapeID + ", " + direction + ") ";
        }*/
    }


    private void PlayerStatsChangeReaction(object sender, EventArgs e) {
        healthBar.UpdateHealth();
        UpdatePlayerPosition(playerStats.currentCellData);
    }

    private void OnDisable() {
        GameEventSystem.instance.OnPlayerStatsChange -= PlayerStatsChangeReaction;
    }
}
