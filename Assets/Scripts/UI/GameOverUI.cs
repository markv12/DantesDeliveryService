using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    public RectTransform bgTransform;
    public TMP_Text highScoreLabel;
    public TMP_Text totalEarnedLabel;
    public Button restartButton;

    private void Awake() {
        restartButton.onClick.AddListener(Restart);
    }

    public void Show() {
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
                highScoreLabel.text = GetHighScoreText(recordData);
            }
        }));

        totalEarnedLabel.text = "$" + StatsManager.instance.TotalMoney;
    }

    private string GetHighScoreText(RecordData recordData) {
        string result = "";
        if(recordData.regionRank > 0) {
            result += "Top " + recordData.regionRank + " in " + recordData.region + Environment.NewLine;
        }
        if(recordData.countryRank > 0) {
            result += "Top " + recordData.countryRank + " in " + recordData.country + Environment.NewLine;
        }
        if (recordData.worldRank > 0) {
            result += "Top " + recordData.worldRank + " Worldwide";
        }
        return result;
    }

    private void Restart() {
        AudioManager.Instance.FadeOutBGM();
        PauseManager.ReleaseAllPauses();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
