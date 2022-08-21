using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUpProperties/HealthEffect")]
public class PickUp_HealthEffect : PickUpEffect {
    [Space]
    [Header("Effect specific fields:")]
    [SerializeField]
    [Tooltip("If ticked, amount should be in interval (0, 100]")]
    private bool percentageHeal;
    [SerializeField]
    private float amount;

    public override void ApplyPickUpEffect(GameObject player) {
        float playerMaxHealth = player.GetComponent<PlayerStats>().maxHealth;
        float healingDone;

        if (percentageHeal) {
            healingDone = playerMaxHealth * (amount / 100);
        } else {
            healingDone = amount;
        }

        GameEventSystem.instance.PlayerHealthAffected(healingDone, false);
    }
}
