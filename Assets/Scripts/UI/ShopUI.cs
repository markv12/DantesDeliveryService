using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour {
    public RectTransform bgTransform;
    public Button continueButton;

    private void Awake() {
        continueButton.onClick.AddListener(Continue);
    }

    private Action onComplete;
    public void Show(Action _onComplete) {
        onComplete = _onComplete;
        bgTransform.anchoredPosition = new Vector2(0, 0);
    }

    private void Continue() {
        AudioManager.Instance.PlayUIClick();
        AudioManager.Instance.FadeOutBGM();
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, NightStartUI.OFF_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, onComplete);
    }
}
