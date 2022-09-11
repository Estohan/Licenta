using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InGameUI {
    public class FullmapWindow : MonoBehaviour {
        public static FullmapWindow instance;
        [SerializeField]
        private float zoomIncrementValue;
        [SerializeField]
        private float maxCameraSize;
        [SerializeField]
        private float minCameraSize;
        [SerializeField]
        private Camera fullMapCamera;
        [SerializeField]
        private GameObject player;

        private WaitForEndOfFrame waitForEndOfFrame;
        private bool panningMapCamera;

        private void Awake() {
            if (instance != null && instance != this) {
                // Debug.LogError("Duplicate instance of FullmapWindow.\n");
                Destroy(gameObject);
            } else {
                instance = this;
            }

            Hide();
        }

        private void Start() {
            Vector3 fullMapCameraPos = fullMapCamera.transform.position;
            Vector3 playerPos = player.transform.position;
            fullMapCamera.transform.position = new Vector3(playerPos.x, fullMapCameraPos.y, playerPos.z);

            panningMapCamera = false;
            waitForEndOfFrame = new WaitForEndOfFrame();
        }

        public static void Show() {
            instance.gameObject.SetActive(true);
        }

        public static void Hide() {
            instance.gameObject.SetActive(false);
        }

        // Changes camera size to values in the interval [minCameraSize, maxCameraSize]
        // (recommended interval is [28, 256])
        public void Zoom(float offset) {
            float newCameraSize = fullMapCamera.orthographicSize + (offset * -zoomIncrementValue);
            if (newCameraSize >= minCameraSize && newCameraSize <= maxCameraSize) {
                fullMapCamera.orthographicSize = newCameraSize;
            }
        }

        public void PanMapCamera(bool panning) {
            if (panning) {
                if (!panningMapCamera) {
                    panningMapCamera = true;
                    StartCoroutine(PanMapCoroutine());
                } 
            } else {
                panningMapCamera = false;
            }
        }

        private IEnumerator PanMapCoroutine() {
            Vector3 fullMapCameraPos = fullMapCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 offset;
            while(panningMapCamera) {
                offset = fullMapCameraPos - fullMapCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                fullMapCamera.transform.position += offset;
                yield return waitForEndOfFrame;
            }
        }
    }
}