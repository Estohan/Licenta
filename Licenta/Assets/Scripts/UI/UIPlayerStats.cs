using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerStats : MyMonoBehaviour {

    // Health bar data
    [SerializeField]
    private Image frontHealthBar;
    [SerializeField]
    private Image backHealthBar;
    [SerializeField]
    private float chipSpeed;
    [SerializeField]
    private Color backColorDamage;
    [SerializeField]
    private Color backColorHeal;
    [SerializeField]
    private Image healthIcon;
    [SerializeField]
    private float warningFlickerFreq;
    [SerializeField]
    private float warningThreshold;
    [SerializeField]
    private TextMeshProUGUI currentHealthText;
    [SerializeField]
    private TextMeshProUGUI maxHealthText;
    private float fillAmountFront;
    private float fillAmountBack;
    private float healthFraction;
    private float lerpTimer;
    private float percentageComplete;
    private bool healthUpdateInProgress;
    private bool warningInProgress;
    private bool newHealthUpdate;
    private bool lowHealthWarning;
    private WaitForSeconds waitWarningFlicker;

    public PlayerStats playerStats;

    //public GameObject healthObject;
    public GameObject positionObject;

    // private TextMeshProUGUI healthText;
    private TextMeshProUGUI positionText;


    //private float displayedHealthValue;
    //MazeCoords displayedPlayerPosition;

    private void Awake() {
        // healthText = healthObject.GetComponent<TextMeshProUGUI>();
        positionText = positionObject.GetComponent<TextMeshProUGUI>();
    }

    protected override void Start() {
        base.Start();
        /*if (healthText != null) {
            UpdateHealth(playerStats.maxHealth);
        }*/
        healthUpdateInProgress = false;
        newHealthUpdate = false;
        lowHealthWarning = false;
        warningInProgress = false;

        waitWarningFlicker = new WaitForSeconds(warningFlickerFreq);

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
        CellStats newCurrentCellStats = newCurrentCellData.cellStats;
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
        }
    }

    private void UpdateHealth(float newValue) {
        currentHealthText.text = playerStats.currHealth.ToString();
        maxHealthText.text = playerStats.maxHealth.ToString();
        if (!healthUpdateInProgress) {
            Debug.Log("Started coroutine");
            StartCoroutine(HealthUpdateCoroutine());
        } else {
            newHealthUpdate = true;
        }

        lowHealthWarning = (playerStats.currHealth < warningThreshold * playerStats.maxHealth);

        if (lowHealthWarning && !warningInProgress) {
            StartCoroutine(LowHPWarningCoroutine());
        }
        /*Debug.Log("Update to " + playerStats.currHealth);
        //displayedHealthValue = newValue;
        //healthText.text = newValue.ToString();
        lerpTimer = 0f;
        float healthFraction = playerStats.currHealth / playerStats.maxHealth;
        if (fillAmountBack > healthFraction) { // damage
            frontHealthBar.fillAmount = healthFraction;
            backHealthBar.color = backColorDamage;
            lerpTimer += Time.deltaTime;
            float percentageComplete = lerpTimer / chipSpeed;
            percentageComplete = percentageComplete * percentageComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillAmountBack, healthFraction, percentageComplete);
        }
        if (fillAmountFront < healthFraction) { // heal
            backHealthBar.color = backColorHeal;
            backHealthBar.fillAmount = healthFraction;
            lerpTimer += Time.deltaTime;
            float percentageComplete = lerpTimer / chipSpeed;
            percentageComplete = percentageComplete * percentageComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillAmountFront, backHealthBar.fillAmount, percentageComplete);
        }*/
    }

    private IEnumerator HealthUpdateCoroutine() {
        lerpTimer = 0f;
        healthUpdateInProgress = true;
        fillAmountFront = frontHealthBar.fillAmount;
        fillAmountBack = backHealthBar.fillAmount;
        // percentageComplete = 0f;
        healthFraction = playerStats.currHealth / playerStats.maxHealth;
        while (fillAmountBack > healthFraction || fillAmountFront < healthFraction) {
            //while(percentageComplete < 1f) {
            fillAmountFront = frontHealthBar.fillAmount;
            fillAmountBack = backHealthBar.fillAmount;
            healthFraction = playerStats.currHealth / playerStats.maxHealth;
            if (newHealthUpdate) {
                lerpTimer = 0f;
                newHealthUpdate = false;
            }
            if (fillAmountBack > healthFraction) {
                frontHealthBar.fillAmount = healthFraction;
                backHealthBar.color = backColorDamage;
                /*fillIncrement = (fillAmountBack - healthFraction) / Time.deltaTime;
                backHealthBar.fillAmount -= fillIncrement;*/
                lerpTimer += Time.deltaTime;
                percentageComplete = lerpTimer / chipSpeed;
                percentageComplete = percentageComplete * percentageComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillAmountBack, healthFraction, percentageComplete);
                Debug.Log("\t took damage " + fillAmountBack + " " + healthFraction + " - " + lerpTimer + " - " + percentageComplete);
            }
            if (fillAmountFront < healthFraction) {
                backHealthBar.color = backColorHeal;
                backHealthBar.fillAmount = healthFraction;
                /*fillIncrement = (fillAmountBack - fillAmountFront) / Time.deltaTime;
                frontHealthBar.fillAmount += fillIncrement;*/
                lerpTimer += Time.deltaTime;
                percentageComplete = lerpTimer / chipSpeed;
                percentageComplete = percentageComplete * percentageComplete;
                frontHealthBar.fillAmount = Mathf.Lerp(fillAmountFront, backHealthBar.fillAmount, percentageComplete);
                Debug.Log("\t healed " + fillAmountFront + " " + healthFraction + " - " + lerpTimer + " - " + percentageComplete);
            }
            yield return new WaitForEndOfFrame();
        }
        healthUpdateInProgress = false;
        Debug.Log("Ended coroutine");
    }

    private IEnumerator LowHPWarningCoroutine() {
        warningInProgress = true;
        Color oldColor = new Color(healthIcon.color.r, healthIcon.color.g, healthIcon.color.b, healthIcon.color.a);
        Color newColor = new Color(healthIcon.color.r, healthIcon.color.g, healthIcon.color.b, 0.2f);
        while (lowHealthWarning) {
            healthIcon.color = newColor;
            yield return waitWarningFlicker;
            healthIcon.color = oldColor;
            yield return waitWarningFlicker;
        }
        warningInProgress = false;
    }

    private void PlayerStatsChangeReaction(object sender, EventArgs e) {
        //float damage = displayedHealthValue - playerStats.currHealth;
        /*if(damage > 0) {
            UpdateHealth(playerStats.currHealth);
        }*/
        UpdateHealth(playerStats.currHealth);
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
