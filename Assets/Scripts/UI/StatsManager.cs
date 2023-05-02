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
    [NonSerialized] public int pistolAutoRateLvl = 0;
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
            return 5 + (deliveryMoneyLvl * 2);
        }
    }

    public float PistolShotTime {
        get {
            return Mathf.Lerp(0.4f, 0.075f, pistolAutoRateLvl / (float)POWER_UP_MAX_LVL);
        }
    }

    public float ThrowPower {
        get {
            return Mathf.Lerp(900f, 1700f, throwPowerLvl / (float)POWER_UP_MAX_LVL);
        }
    }
    public float ShotgunReloadTime {
        get {
            return Mathf.Lerp(12, 1.75f, shotgunSpeedLvl / (float)POWER_UP_MAX_LVL);
        }
    }

    public float RunSpeed {
        get {
            return Mathf.Lerp(10, 20f, runSpeedLvl / (float)POWER_UP_MAX_LVL);
        }
    }
}
