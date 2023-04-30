using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightManager : MonoBehaviour {
    public RectTransform dayBar;
    public RectTransform nightBar;
    public RectTransform currentTimeIndicator;
    public RectTransform timeBar;
    private float timeBarWidth;

    public Color daySkyboxColor;
    public Color nightSkyboxColor;
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

    private bool isNight = false;
    private bool IsNight {
        get {
            return isNight;
        }
        set {
            isNight = value;
            SetSwapSprites(isNight);
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
            FadeSkyboxColor(nightSkyboxColor);
            currentTime = 0;

            yield return WaitUtil.GetWait(2);
            nightStartUI.Show(() => {
                IsNight = true;
                PauseManager.RequestPause(this);
                Player.instance.SetFPSControllerActive(false);
            }, () => {
                PauseManager.ReleasePause(this);
                Player.instance.SetFPSControllerActive(true);
                changing = false;
            });
        }
    }

    private void ChangeToDay() {
        StartCoroutine(DayChangeRoutine());

        IEnumerator DayChangeRoutine() {
            changing = true;
            FadeSkyboxColor(daySkyboxColor);
            currentTime = 0;

            yield return WaitUtil.GetWait(2);
            dayStartUI.Show(() => {
                IsNight = false;
                Player.instance.SetFPSControllerActive(false);
            }, () => {
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
