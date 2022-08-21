using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour {
    public static GameEventSystem instance = null;

    public delegate void PlayerHealthAffectedDelegate(object sender, float amount, bool onMaxHealth);
    public delegate void PlayerDeathDelegate(object sender);
    // public delegate void PlayerMoveToCellDelegate(object sender, MazeCellData cellData);

    public event PlayerHealthAffectedDelegate OnHealthAffected;
    public event PlayerDeathDelegate OnPlayerDeath;
    // public event PlayerMoveToCellDelegate OnPlayerMoveToAnotherCell;
    public event EventHandler OnPlayerStatsChange;
    // public event EventHandler OnPlayerHit;

    private void Awake() {
        if (instance != null && instance != this) {
            Debug.LogError("Duplicate instance of GameEventSystem.\n");
            Destroy(gameObject);
        } else {
            instance = this;
        }
    }

    public void PlayerHealthAffected(float amount, bool onMaxHealth) {
        // OnPlayerHit?.Invoke(this, EventArgs.Empty);
        OnHealthAffected?.Invoke(this, amount, onMaxHealth);
    }

    public void PlayerDeath() {
        OnPlayerDeath?.Invoke(this);
    }

    public void PlayerStatsChanged() {
        OnPlayerStatsChange?.Invoke(this, EventArgs.Empty);
    }

    // [ TODO ] [Debug] Do I need this?
    /*public void PlayerMoveedToAnotherCell(MazeCellData cellData) {
        OnPlayerMoveToAnotherCell?.Invoke(this, cellData);
    }*/
}
