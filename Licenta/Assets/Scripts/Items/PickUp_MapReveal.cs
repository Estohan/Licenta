using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PickUpProperties/MapRevealEffect")]
public class PickUp_MapReveal : PickUpEffect {
    [Space]
    [Header("Effect specific fields:")]
    [SerializeField]
    private bool onCurrentLevel;
    [SerializeField]
    private bool destinationReveal;


    public override void ApplyPickUpEffect(GameObject player) {
        if (onCurrentLevel) {
            if (destinationReveal) {
                // Reveal destination cell on current level
                GameManager.instance.ExecuteLevelEffect(LevelEffectsManager.LevelEffects.DestinationReveal);
            } else {
                // Reveal map portion on current level
                GameManager.instance.ExecuteLevelEffect(LevelEffectsManager.LevelEffects.MapReveal);
            }
        } else {
            if (destinationReveal) {
                // Reveal destination on next level
                GameManager.instance.AddEffectToQueue(LevelEffectsManager.LevelEffects.DestinationReveal);
            } else {
                // Reveal map portion on next level
                GameManager.instance.AddEffectToQueue(LevelEffectsManager.LevelEffects.MapReveal);
            }
        }
    }


}
