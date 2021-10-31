using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class MinimapWindow : MonoBehaviour {

        private static MinimapWindow instance;

        private void Awake() {
            if (instance != null && instance != this) {
                Debug.LogError("Duplicate instance of MinimapWindow.\n");
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }
        
        public static void Show() {
            instance.gameObject.SetActive(true);
        }
        public static void Hide() {
            instance.gameObject.SetActive(false);
        }
    }
}