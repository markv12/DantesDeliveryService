using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NightStartUI : MonoBehaviour {
    public RectTransform bgTransform;
    public RectTransform characterT;
    public Image characterImage;
    public Sprite dayCharacterSprite;
    public Sprite nightCharacterSprite;
    public ShopUI shopUI;

    public static readonly Vector2 ON_SCREEN_POS = new Vector2(0, 0);
    public static readonly Vector2 OFF_SCREEN_POS = new Vector2(0, 1080);

    private void Awake() {
        SetCharacterNightMode(false);
    }

    public void Show(Action onCoverScreen, Action onComplete) {
        Vector2 startPos = bgTransform.anchoredPosition;
        Player.instance.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, ON_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, () => {
            onCoverScreen?.Invoke();
            shopUI.Show(onComplete);
            Continue();
        });
    }

    private void Continue() {
        Player.instance.StartCoroutine(ContinueRoutine());
        IEnumerator ContinueRoutine() {
            yield return new WaitForSecondsRealtime(0.3f);
            yield return Player.instance.CreateAnimationRoutine(0.5f, (float progress) => {
                characterT.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 90, progress), 0);
            });

            SetCharacterNightMode(true);

            yield return Player.instance.CreateAnimationRoutine(0.5f, (float progress) => {
                characterT.localEulerAngles = new Vector3(0, Mathf.Lerp(90, 0, progress), 0);
            });
            yield return new WaitForSecondsRealtime(0.3f);
            AudioManager.Instance.PlayShopTheme();
            Vector2 startPos = bgTransform.anchoredPosition;
            Player.instance.CreateAnimationRoutine(1f, (float progress) => {
                bgTransform.anchoredPosition = Vector2.Lerp(startPos, OFF_SCREEN_POS, Easing.easeInSine(0, 1, progress));
            }, () => {
                SetCharacterNightMode(false);
            });
        }
    }

    public static readonly Vector2 NIGHT_SPRITE_SIZE = new Vector2(712, 900);
    public static readonly Vector2 DAY_SPRITE_SIZE = new Vector2(425, 715);
    private void SetCharacterNightMode(bool isNight) {
        characterT.sizeDelta = isNight ? NIGHT_SPRITE_SIZE : DAY_SPRITE_SIZE;
        characterImage.sprite = isNight ? nightCharacterSprite : dayCharacterSprite;
    }
}
