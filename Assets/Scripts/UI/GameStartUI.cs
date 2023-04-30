using UnityEngine;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour {
    public RectTransform bgTransform;
    public Button startButton;

    private void Awake() {
        bgTransform.anchoredPosition = NightStartUI.ON_SCREEN_POS;
        startButton.onClick.AddListener(StartGame);
    }

    private void Start() {
        Player.instance.SetFPSControllerActive(false);
    }

    private void StartGame() {
        AudioManager.Instance.PlayUIClick();
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, NightStartUI.OFF_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, () => {
            Player.instance.SetFPSControllerActive(true);
        });
    }
}
