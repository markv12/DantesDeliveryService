using System;
using UnityEngine;
using UnityEngine.UI;

public class NightStartUI : MonoBehaviour
{
    public RectTransform bgTransform;
    public Button continueButton;

    private static readonly Vector2 ON_SCREEN_POS = new Vector2(0, 0);
    private static readonly Vector2 OFF_SCREEN_POS = new Vector2(0, 1080);

    private void Awake() {
        continueButton.onClick.AddListener(Continue);
    }

    private Action onComplete;
    public void Show(Action onCoverScreen, Action _onComplete) {
        onComplete = _onComplete;
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, ON_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, onCoverScreen);
    }

    private void Continue() {
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, OFF_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, () => {
            onComplete?.Invoke();
        });
    }
}
