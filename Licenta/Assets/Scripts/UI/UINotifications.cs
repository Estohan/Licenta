using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 *      UI notifications system.
 */
namespace InGameUI {
    public class UINotifications : MonoBehaviour {

        public static UINotifications instance;

        [SerializeField]
        private GameObject largeBanner;
        [SerializeField]
        private TextMeshProUGUI largeBannerText;
        [SerializeField]
        private GameObject smallBanner;
        [SerializeField]
        private TextMeshProUGUI smallBannerText;
        [Space]
        [SerializeField]
        private float largeDisplayTime;
        [SerializeField]
        private float smallDisplayTime;
        [SerializeField]
        private float largeInBetweeenTime;
        [SerializeField]
        private float smallInBetweeenTime;

        private Queue<string> largeNotifQueue;
        private Queue<string> smallNotifQueue;

        private WaitForSeconds KeepLargeNotifForSeconds;
        private WaitForSeconds KeepSmallNotifFOrSeconds;
        private WaitForSeconds WaitSecondsBetweenLarge;
        private WaitForSeconds WaitSecondsBetweenSmall;

        private bool largeCurrentlyOnScreen;
        private bool smallCurrentlyOnScreen;

        private void Awake() {
            if (instance != null && instance != this) {
                // Debug.LogError("Duplicate instance of Notifications.\n");
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }

        private void Start() {
            largeBanner.SetActive(false);
            smallBanner.SetActive(false);

            largeNotifQueue = new Queue<string>();
            smallNotifQueue = new Queue<string>();

            KeepLargeNotifForSeconds = new WaitForSeconds(largeDisplayTime);
            KeepSmallNotifFOrSeconds = new WaitForSeconds(smallDisplayTime);
            WaitSecondsBetweenLarge = new WaitForSeconds(largeInBetweeenTime);
            WaitSecondsBetweenSmall = new WaitForSeconds(smallInBetweeenTime);

            largeCurrentlyOnScreen = false;
            smallCurrentlyOnScreen = false;
        }

        public void DisplayNotification(string notificationText, bool large = false) {
            // Debug.Log("Display \"" + notificationText + "\", " + large);
            // Small banner notifications
            if (!large) {
                smallNotifQueue.Enqueue(notificationText);
                if (!smallCurrentlyOnScreen) {
                    StartCoroutine(SmallNotifCoroutine());
                }
            // Large banner notifications
            } else {
                largeNotifQueue.Enqueue(notificationText);
                if (!largeCurrentlyOnScreen) {
                    StartCoroutine(LargeNotifCoroutine());
                }
            }
        }

        public void StopAndDiscardNotifications() {
            largeNotifQueue.Clear();
            smallNotifQueue.Clear();
            if (largeCurrentlyOnScreen) {
                StopCoroutine(LargeNotifCoroutine());
                largeBanner.SetActive(false);
            }
            if (smallCurrentlyOnScreen) {
                StopCoroutine(SmallNotifCoroutine());
                smallBanner.SetActive(false);
            }
            largeCurrentlyOnScreen = false;
            smallCurrentlyOnScreen = false;
        }

        private IEnumerator LargeNotifCoroutine() {
            largeCurrentlyOnScreen = true;
            while (largeNotifQueue.Count > 0) {
                largeBanner.SetActive(true);
                largeBannerText.text = largeNotifQueue.Dequeue();
                yield return KeepLargeNotifForSeconds;
                largeBanner.SetActive(false);
                yield return WaitSecondsBetweenLarge;
            }
            largeCurrentlyOnScreen = false;
        }

        private IEnumerator SmallNotifCoroutine() {
            smallCurrentlyOnScreen = true;
            while (smallNotifQueue.Count > 0) {
                smallBanner.SetActive(true);
                smallBannerText.text = smallNotifQueue.Dequeue();
                yield return KeepSmallNotifFOrSeconds;
                smallBanner.SetActive(false);
                yield return WaitSecondsBetweenSmall;
            }
            smallCurrentlyOnScreen = false;
        }

    }
}
