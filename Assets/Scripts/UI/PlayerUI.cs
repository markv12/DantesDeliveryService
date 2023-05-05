using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private MinimapCamera minimapCamera;
    [SerializeField] private GameObject smallMap;
    [SerializeField] private GameObject pauseMapUI;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private TMP_Text healthText;

    private void Start() {
        DayNightManager.instance.IsNightChanged += HandleIsNightChanged;
    }

    private void HandleIsNightChanged(bool isNight) {
        healthUI.SetActive(isNight);
    }
    
    public void ShowHealth(int health, int maxHealth) {
        healthText.text = health + "/" + maxHealth;
        healthBar.sizeDelta = healthBar.sizeDelta.SetX(Mathf.Lerp(0, 366f, health / (float)maxHealth));
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
