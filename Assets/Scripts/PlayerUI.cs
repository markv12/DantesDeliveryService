using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private RectTransform throwMeter;
    [SerializeField] private MinimapCamera minimapCamera;
    [SerializeField] private GameObject smallMap;
    [SerializeField] private GameObject pauseMapUI;

    public void ShowThrowStrength(float throwStrength) {
        throwMeter.sizeDelta = new Vector2(60, 300f * throwStrength);
    }

    private void Update() {
        if (pauseMapOpen) {
            if (InputUtil.GetKeyDown(Key.M) || InputUtil.GetKeyDown(Key.Escape)) {
                SetPauseMapOpen(false);
            }
        } else {
            if (InputUtil.GetKeyDown(Key.M) || InputUtil.GetKeyDown(Key.Escape)) {
                SetPauseMapOpen(true);
            }
        }
    }

    private bool pauseMapOpen = false;
    private void SetPauseMapOpen(bool open) {
        pauseMapOpen = open;
        smallMap.SetActive(!open);
        pauseMapUI.SetActive(open);
 
        minimapCamera.FollowPlayerMode = !open;
        if(Player.instance != null) {
            Player.instance.SetFPSControllerActive(!open);
        }
    }
}
