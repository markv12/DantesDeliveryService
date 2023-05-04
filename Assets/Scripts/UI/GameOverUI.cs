using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    public RectTransform bgTransform;
    public TMP_Text highScoreLabelRegion;
    public TMP_Text highScoreLabelCountry;
    public TMP_Text highScoreLabelWorldwide;
    public TMP_Text totalEarnedLabel;
    public Button showRegionButton;
    public Button showCountryButton;
    public Button restartButton;

    private void Awake() {
        restartButton.onClick.AddListener(Restart);
        showRegionButton.onClick.AddListener(() => {
            highScoreLabelRegion.gameObject.SetActive(true);
            showRegionButton.gameObject.SetActive(false);
        });
        showCountryButton.onClick.AddListener(() => {
            highScoreLabelCountry.gameObject.SetActive(true);
            showCountryButton.gameObject.SetActive(false);
        });
    }

    public void Show() {
        highScoreLabelRegion.gameObject.SetActive(false);
        highScoreLabelCountry.gameObject.SetActive(false);

        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1.5f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, NightStartUI.ON_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, () => {
            AudioManager.Instance.PlayShopTheme();
        });
        ValueTuple<string, string>[] bodyParams = new ValueTuple<string, string>[]{
            ("score", StatsManager.instance.TotalMoney.ToString()),
        };
        StartCoroutine(NetUtility.Post("https://p.jasperstephenson.com/ld53/score/add", bodyParams, (bool sadf, string result) => {
            RecordData recordData = RecordData.CreateFromJsonString(result);
            if(recordData != null) {
                highScoreLabelRegion.text = "Top " + recordData.regionRank + " in " + recordData.region;
                highScoreLabelCountry.text = "Top " + recordData.countryRank + " in " + recordData.country;
                highScoreLabelWorldwide.text = "Top " + recordData.worldRank + " Worldwide";
            }
        }));

        totalEarnedLabel.text = "$" + StatsManager.instance.TotalMoney;
    }

    private void Restart() {
        AudioManager.Instance.FadeOutBGM();
        PauseManager.ReleaseAllPauses();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
