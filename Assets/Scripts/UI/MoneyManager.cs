using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour {
    public TMP_Text moneyLabel;

    public static MoneyManager instance;

    private void Awake() {
        instance = this;
    }

    private int currentMoney = 0;
    public int CurrentMoney {
        get {
            return currentMoney;
        }
        set {
            currentMoney = value;
            moneyLabel.text = "$" + currentMoney.ToString();
        }
    }
}
