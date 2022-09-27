using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Abstact class inherited by all classes that represent effects of PickUp
 *  game objects.
 */
public abstract class PickUpEffect : ScriptableObject {
    public abstract void ApplyPickUpEffect(GameObject player);
}
