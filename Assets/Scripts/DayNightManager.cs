using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightManager : MonoBehaviour {
    public RectTransform dayBar;
    public RectTransform nightBar;
    public RectTransform currentTimeIndicator;
    public RectTransform timeBar;
    private float timeBarWidth;

    public TMP_Text dayNightLabel;
    public TMP_Text dayCountLabel;

    public Color daySkyboxColor;
    public Color nightSkyboxColor;
    public Texture2D daySkyboxTexture;
    public Texture2D nightSkyboxTexture;

    public NightStartUI nightStartUI;
    public DayStartUI dayStartUI;

    public static DayNightManager instance;
    private readonly List<DayNightSpriteSwap> spriteSwaps = new List<DayNightSpriteSwap>(64);

    private float currentTime = 0;
    private bool changing = false;
    [Min(0)]
    public float dayLength = 2;
    [Min(0)]
    public float nightLength = 2;
    private float totalLength = 2;

    void Awake() {
        instance = this;
        totalLength = dayLength + nightLength;
        timeBarWidth = timeBar.sizeDelta.x;
        float nightX = (dayLength / totalLength) * timeBarWidth;
        dayBar.sizeDelta = new Vector2(nightX + 5, 30);
        nightBar.anchoredPosition = new Vector2(nightX, 0);
        nightBar.sizeDelta = new Vector2(timeBarWidth - nightX, 30);
        RenderSettings.skybox = new Material(RenderSettings.skybox);
    }

    private int currentDay = 1;
    private int CurrentDay {
        get {
            return currentDay;
        }
        set {
            currentDay = value;
            dayCountLabel.text = currentDay.ToString();
        }
    }

    public event Action<bool> IsNightChanged;
    private bool isNight = false;
    public bool IsNight {
        get {
            return isNight;
        }
        private set {
            if(isNight != value) {
                isNight = value;
                IsNightChanged?.Invoke(isNight);
                SetSwapSprites(isNight);
                dayNightLabel.text = isNight ? "Night" : "Day";
                Material skyboxMat = RenderSettings.skybox;
                skyboxMat.SetColor("_Tint", daySkyboxColor);
                skyboxMat.SetTexture("_MainTex", isNight ? nightSkyboxTexture : daySkyboxTexture);
            }
        }
    }

    private void Update() {
        if (!changing) {
            currentTime += Time.deltaTime;
            float totalTime = isNight ? dayLength + currentTime : currentTime;
            float currTimeX = (totalTime / totalLength) * timeBarWidth;
            currentTimeIndicator.anchoredPosition = new Vector2(currTimeX, -10);

            if (isNight) {
                if (currentTime >= nightLength) {
                    ChangeToDay();
                }
            } else {
                if (currentTime >= dayLength) {
                    ChangeToNight();
                }
            }
        }
    }

    private void ChangeToNight() {
        StartCoroutine(ChangeRoutine());

        IEnumerator ChangeRoutine() {
            changing = true;
            AudioManager.Instance.FadeOutBGM();
            AudioManager.Instance.PlayNightStart();

            FadeSkyboxColor(nightSkyboxColor);
            currentTime = 0;

            yield return WaitUtil.GetWait(2);
            nightStartUI.Show(() => {
                IsNight = true;
                Player.instance.SetFPSControllerActive(false);
            }, () => {
                AudioManager.Instance.PlayNightTheme();
                Player.instance.SetFPSControllerActive(true);
                changing = false;
            });
        }
    }

    private void ChangeToDay() {
        StartCoroutine(DayChangeRoutine());

        IEnumerator DayChangeRoutine() {
            changing = true;
            AudioManager.Instance.FadeOutBGM();
            AudioManager.Instance.PlayDayStart();
            FadeSkyboxColor(daySkyboxColor);
            currentTime = 0;

            yield return WaitUtil.GetWait(2);
            dayStartUI.Show(() => {
                IsNight = false;
                CurrentDay++;
                Player.instance.SetFPSControllerActive(false);
                Player.instance.PutGunsAway();
            }, () => {
                AudioManager.Instance.PlayDayTheme();
                Player.instance.SetFPSControllerActive(true);
                changing = false;
            });
        }
    }

    private void FadeSkyboxColor(Color color) {
        Material skyboxMat = RenderSettings.skybox;
        Color startColor = skyboxMat.GetColor("_Tint");
        this.CreateAnimationRoutine(3f, (float progress) => {
            skyboxMat.SetColor("_Tint", Color.Lerp(startColor, color, progress));
        });
    }

    public void Register(DayNightSpriteSwap swap) {
        spriteSwaps.Add(swap);
    }
    public void Unregister(DayNightSpriteSwap swap) {
        spriteSwaps.Remove(swap);
    }
    private void SetSwapSprites(bool isNight) {
        for (int i = 0; i < spriteSwaps.Count; i++) {
            DayNightSpriteSwap spriteSwap = spriteSwaps[i];
            spriteSwap.mainRenderer.sprite = isNight ? spriteSwap.nightSprite : spriteSwap.daySprite;
        }
    }
}
