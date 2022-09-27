using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Event system used for the interactions between the character and
 *  obstacles.
 */
public class GameEventSystem : MonoBehaviour {
    public static GameEventSystem instance = null;

    public delegate void PlayerHealthAffectedDelegate(object sender, float amount, bool onMaxHealth);
    public delegate void PlayerDeathDelegate(object sender);
    // public delegate void PlayerMoveToCellDelegate(object sender, MazeCellData cellData);

    public event PlayerHealthAffectedDelegate OnHealthAffected;
    public event PlayerDeathDelegate OnPlayerDeath;
    // public event PlayerMoveToCellDelegate OnPlayerMoveToAnotherCell;
    public event EventHandler OnPlayerStatsChange;

    private void Awake() {
        if (instance != null && instance != this) {
            // Debug.LogError("Duplicate instance of GameEventSystem.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public void PlayerHealthAffected(float amount, bool onMaxHealth) {
        OnHealthAffected?.Invoke(this, amount, onMaxHealth);
    }

    public void PlayerDeath() {
        OnPlayerDeath?.Invoke(this);
    }

    public void PlayerStatsChanged() {
        OnPlayerStatsChange?.Invoke(this, EventArgs.Empty);
    }

    // [Debug]
    /*public void PlayerMoveedToAnotherCell(MazeCellData cellData) {
        OnPlayerMoveToAnotherCell?.Invoke(this, cellData);
    }*/
}
