using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerStats : MyMonoBehaviour {
    public GameObject healthObject;
    public PlayerStats playerStats;

    private TextMeshProUGUI healthText;
    private float displayedHealthValue;

    private void Awake() {
        healthText = healthObject.GetComponent<TextMeshProUGUI>();
    }

    protected override void Start() {
        base.Start();
        if (healthText != null) {
            UpdateHealth(playerStats.maxHealth);
        } else {
            Debug.Log("Uff");
        }
    }

    private void UpdateHealth(float newValue) {
        displayedHealthValue = newValue;
        healthText.text = newValue.ToString();
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void SafeOnEnable() {
        // events
        GameEventSystem.instance.OnPlayerStatsChange += PlayerStatsChangeReaction;
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

    private void PlayerStatsChangeReaction(object sender, EventArgs e) {
        float damage = displayedHealthValue - playerStats.currHealth;
        Debug.Log("UI Player Stats: Player was hit for " + damage + " damage!");
        UpdateHealth(playerStats.currHealth);
    }

    private void OnDisable() {
        GameEventSystem.instance.OnPlayerStatsChange -= PlayerStatsChangeReaction;
    }
}
