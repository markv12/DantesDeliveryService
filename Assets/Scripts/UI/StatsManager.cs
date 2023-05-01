using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour {
    public TMP_Text moneyLabel;
    public TMP_Text shopMoneyLabel;

    public static StatsManager instance;

    private void Awake() {
        instance = this;
    }

    public int TotalMoney { get; private set; }
    private int currentMoney = 0;
    public int CurrentMoney {
        get {
            return currentMoney;
        }
        private set {
            currentMoney = value;
            moneyLabel.text = "$" + currentMoney.ToString();
            shopMoneyLabel.text = "$" + currentMoney.ToString();
        }
    }

    public void AddMoney(int amount) {
        CurrentMoney += amount;
        TotalMoney += amount;
    }

    public void SpendMoney(int amount) {
        CurrentMoney -= amount;
    }
}
