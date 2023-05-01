using System;
using System.Collections;
using UnityEngine;

public class NightStartUI : MonoBehaviour {
    public RectTransform bgTransform;
    public ShopUI shopUI;

    public static readonly Vector2 ON_SCREEN_POS = new Vector2(0, 0);
    public static readonly Vector2 OFF_SCREEN_POS = new Vector2(0, 1080);

    public void Show(Action onCoverScreen, Action onComplete) {
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, ON_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, () => {
            onCoverScreen?.Invoke();
            shopUI.Show(onComplete);
            Continue();
        });
    }

    private void Continue() {
        StartCoroutine(ContinueRoutine());
        IEnumerator ContinueRoutine() {
            yield return new WaitForSecondsRealtime(1.4f);
            AudioManager.Instance.PlayShopTheme();
            Vector2 startPos = bgTransform.anchoredPosition;
            this.CreateAnimationRoutine(1f, (float progress) => {
                bgTransform.anchoredPosition = Vector2.Lerp(startPos, OFF_SCREEN_POS, Easing.easeInSine(0, 1, progress));
            });
        }
    }
}
