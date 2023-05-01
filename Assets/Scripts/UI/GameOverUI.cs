using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public RectTransform bgTransform;
    public Button restartButton;

    private void Awake() {
        restartButton.onClick.AddListener(Restart);
    }

    public void Show(Action onCoverScreen) {
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1.5f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, NightStartUI.ON_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, () => {
            onCoverScreen?.Invoke();
            AudioManager.Instance.PlayShopTheme();
        });
    }

    private void Restart() {
        AudioManager.Instance.FadeOutBGM();
        PauseManager.ReleaseAllPauses();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
