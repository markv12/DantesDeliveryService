using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayStartUI : MonoBehaviour
{
    public RectTransform bgTransform;

    private Action onComplete;
    public void Show(Action onCoverScreen, Action _onComplete) {
        onComplete = _onComplete;
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, NightStartUI.ON_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, () => {
            onCoverScreen?.Invoke();
            Continue();
        });
    }

    private void Continue() {
        StartCoroutine(ContinueRoutine());
        IEnumerator ContinueRoutine() {
            yield return new WaitForSecondsRealtime(1.4f);
            Vector2 startPos = bgTransform.anchoredPosition;
            this.CreateAnimationRoutine(1f, (float progress) => {
                bgTransform.anchoredPosition = Vector2.Lerp(startPos, NightStartUI.OFF_SCREEN_POS, Easing.easeInSine(0, 1, progress));
            }, onComplete);
        }
    }
}
