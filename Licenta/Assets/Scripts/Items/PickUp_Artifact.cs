using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUpProperties/Artifact")]
public class PickUp_Artifact : PickUpEffect {
    public override void ApplyPickUpEffect(GameObject player) {
        GameManager.instance.EndLevel();
    }
}
