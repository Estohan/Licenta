using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerStats : MonoBehaviour {
    public GameObject healthObject;
    public PlayerStats playerStats;

    private TextMeshProUGUI healthText;

    private void Awake() {
        healthText = healthObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        GameEventSystem.instance.OnPlayerHit += HitPlayerReaction;

        if (healthText != null) {
            healthText.text = playerStats.maxHealth.ToString();
        } else {
            Debug.Log("Uff");
        }
    }

    /*private void HitPlayerReaction(object sender, EventArgs e) {
        Debug.Log("UI Player Stats: Player was hit!");
    }*/

    private void HitPlayerReaction(object sender, float damage) {
        //Debug.Log("UI Player Stats: Player was hit for " + damage + " damage!");
        StartCoroutine(__Wait(damage)); // -------------------------------------------------------------------- <<<<
        //healthText.text = playerStats.currHealth.ToString();
    }

    private IEnumerator __Wait(float damage) {
        yield return new WaitForSeconds(1);
        healthText.text = playerStats.currHealth.ToString();
        Debug.Log("UI Player Stats: Player was hit for " + damage + " damage!");
    }
}
