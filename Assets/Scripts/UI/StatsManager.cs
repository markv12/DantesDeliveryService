using System;
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour {
    public TMP_Text moneyLabel;
    public TMP_Text shopMoneyLabel;

    public static StatsManager instance;

    private void Awake() {
        instance = this;
    }

    public const int POWER_UP_MAX_LVL = 3;
    [NonSerialized] public int pistolDmgLvl = 0;
    [NonSerialized] public bool pistolFullAuto = false;
    [NonSerialized] public int runSpeedLvl = 0;
    [NonSerialized] public bool shotgunUnlocked = false;
    [NonSerialized] public int shotgunSpeedLvl = 0;
    [NonSerialized] public int deliveryMoneyLvl = 0;
    [NonSerialized] public int throwPowerLvl = 0;

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

    public int PistolDamage {
        get {
            return 10 + (pistolDmgLvl * 2);
        }
    }

    public int DelveryMoney {
        get {
            return 10 + (deliveryMoneyLvl * 5);
        }
    }

    public float ThrowPower {
        get {
            return Mathf.Lerp(900f, 1700f, throwPowerLvl / (float)POWER_UP_MAX_LVL);
        }
    }
    public float ShotgunReloadTime {
        get {
            return Mathf.Lerp(2.25f, 1.1f, shotgunSpeedLvl / (float)POWER_UP_MAX_LVL);
        }
    }

    public float RunSpeed {
        get {
            return Mathf.Lerp(10, 20f, runSpeedLvl / (float)POWER_UP_MAX_LVL);
        }
    }
}
