using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map {
    public class FullmapWindow : MonoBehaviour {
        private static FullmapWindow instance;

        private void Awake() {
            if (instance != null && instance != this) {
                Debug.LogError("Duplicate instance of FullmapWindow.\n");
                Destroy(gameObject);
            } else {
                instance = this;
            }

            Hide();
        }

        public static void Show() {
            instance.gameObject.SetActive(true);
        }

        public static void Hide() {
            instance.gameObject.SetActive(false);
        }
    }
}