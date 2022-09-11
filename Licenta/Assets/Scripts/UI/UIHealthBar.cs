using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHealthBar : MonoBehaviour {

    [SerializeField]
    [Tooltip("This will provide the current health and max. health values.")]
    private PlayerStats playerStats;

    [SerializeField]
    private Image healthIcon;
    [SerializeField]
    private Image frontHealthBar;
    [SerializeField]
    private Image backHealthBar;
    [SerializeField]
    private Color backColorDamage;
    [SerializeField]
    private Color backColorHeal;

    [SerializeField]
    [Tooltip("Health bar update animation duration (seconds).")]
    private float chipSpeed;
    [SerializeField]
    [Tooltip("Time interval between two transitions (seconds).")]
    private float warningFlickerFreq;
    [SerializeField]
    [Tooltip("Health percentage threshold.")]
    [Range(0.0f, 1.0f)]
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

    private void Start() {
        healthUpdateInProgress = false;
        newHealthUpdate = false;
        lowHealthWarning = false;
        warningInProgress = false;

        waitWarningFlicker = new WaitForSeconds(warningFlickerFreq);

        // Hide health bar at the start of the game
        this.gameObject.SetActive(false);
    }

    public void UpdateHealth() {
        currentHealthText.text = ((int) playerStats.currHealth).ToString();
        maxHealthText.text = ((int) playerStats.maxHealth).ToString();
        if (!healthUpdateInProgress) {
            StartCoroutine(HealthUpdateCoroutine());
        } else {
            newHealthUpdate = true;
        }

        lowHealthWarning = (playerStats.currHealth < warningThreshold * playerStats.maxHealth);

        if (lowHealthWarning && !warningInProgress) {
            StartCoroutine(LowHPWarningCoroutine());
        }
    }

    private IEnumerator HealthUpdateCoroutine() {
        lerpTimer = 0f;
        healthUpdateInProgress = true;
        fillAmountFront = frontHealthBar.fillAmount;
        fillAmountBack = backHealthBar.fillAmount;
        healthFraction = playerStats.currHealth / playerStats.maxHealth;
        while (fillAmountBack > healthFraction || fillAmountFront < healthFraction) {
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
                lerpTimer += Time.deltaTime;
                percentageComplete = lerpTimer / chipSpeed;
                percentageComplete = percentageComplete * percentageComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillAmountBack, healthFraction, percentageComplete);
                // Debug.Log("\t took damage " + fillAmountBack + " " + healthFraction + " - " + lerpTimer + " - " + percentageComplete);
            }
            if (fillAmountFront < healthFraction) {
                backHealthBar.color = backColorHeal;
                backHealthBar.fillAmount = healthFraction;
                lerpTimer += Time.deltaTime;
                percentageComplete = lerpTimer / chipSpeed;
                percentageComplete = percentageComplete * percentageComplete;
                frontHealthBar.fillAmount = Mathf.Lerp(fillAmountFront, backHealthBar.fillAmount, percentageComplete);
                //Debug.Log("\t healed " + fillAmountFront + " " + healthFraction + " - " + lerpTimer + " - " + percentageComplete);
            }
            yield return new WaitForEndOfFrame();
        }
        healthUpdateInProgress = false;
        //Debug.Log("Ended coroutine");
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
}
