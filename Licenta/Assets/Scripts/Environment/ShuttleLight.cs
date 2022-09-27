using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *      Controlls the light present on the shuttle visible on the first level
 *  (and also in the main menu).
 */
public class ShuttleLight : MonoBehaviour {

    [SerializeField]
    private Light shuttleLight;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float step;
    [SerializeField]
    private float maxIntensity;

    private WaitForSecondsRealtime waitSeconds;

    private void Start() {
        waitSeconds = new WaitForSecondsRealtime(speed);
        shuttleLight.intensity = maxIntensity;
        StartCoroutine(ShuttleLightCoroutine());
    }

    private IEnumerator ShuttleLightCoroutine() {
        bool increaseIntensity = false;
        while(true) {
            if (increaseIntensity) {
                if (shuttleLight.intensity < maxIntensity) {
                    shuttleLight.intensity += step;
                } else {
                    increaseIntensity = false;
                }
            } else {
                if (shuttleLight.intensity - step > 0) {
                    shuttleLight.intensity -= step;
                } else {
                    increaseIntensity = true;
                }
            }
            yield return waitSeconds;
        }
    }

}
