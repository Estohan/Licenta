using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUpProperties/MaxHealthEffect")]
public class PickUp_MaxHealthEffect : PickUpEffect {
    [Space]
    [Header("Effect specific fields:")]
    [SerializeField]
    private float amount;

    public override void ApplyPickUpEffect(GameObject player) {
        GameEventSystem.instance.PlayerHealthAffected(amount, true);
    }
}
