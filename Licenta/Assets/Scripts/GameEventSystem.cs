using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour {
    public static GameEventSystem instance = null;

    public delegate void PlayerHitDelegate(object sender, float damage);
    public delegate void PlayerDeathDelegate(object sender);
    public delegate void PlayerMoveToCellDelegate(object sender, MazeCellData cellData);

    public event PlayerHitDelegate OnPlayerHit;
    public event PlayerDeathDelegate OnPlayerDeath;
    public event PlayerMoveToCellDelegate OnPlayerMoveToAnotherCell;
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

    public void PlayerHit(float damage) {
        // OnPlayerHit?.Invoke(this, EventArgs.Empty);
        OnPlayerHit?.Invoke(this, damage);
    }

    public void PlayerDeath() {
        OnPlayerDeath?.Invoke(this);
    }

    public void PlayerStatsChanged() {
        OnPlayerStatsChange?.Invoke(this, EventArgs.Empty);
    }

    public void PlayerMoveedToAnotherCell(MazeCellData cellData) {
        OnPlayerMoveToAnotherCell?.Invoke(this, cellData);
    }
}
