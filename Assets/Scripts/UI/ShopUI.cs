using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour {
    public RectTransform bgTransform;
    public RectTransform elementContainer;
    public Button continueButton;
    public BuyableElement buyableElementPrefab;
    private readonly List<BuyableElement> currentElements = new List<BuyableElement>(32);

    private void Awake() {
        continueButton.onClick.AddListener(Continue);
    }

    private Action onComplete;
    public void Show(Action _onComplete) {
        Clear();
        SetupItems();
        onComplete = _onComplete;
        bgTransform.anchoredPosition = new Vector2(0, 0);
    }

    private void SetupItems() {
        SetupValidPowerups();
        for (int i = 0; i < 6; i++) {
            if (i < validPowerups.Count) {
                int x = i % 2;
                int y = i / 2;
                BuyableElement newElement = Instantiate(buyableElementPrefab, elementContainer);
                newElement.rectT.anchoredPosition = new Vector2(x * 350, y * -120);

                PowerUpType powerUpType = validPowerups[i];
                newElement.Setup(GetItem(powerUpType));
                currentElements.Add(newElement);
            }
        }
    }

    private readonly List<PowerUpType> validPowerups = new List<PowerUpType>(16);
    private void SetupValidPowerups() {
        validPowerups.Clear();
        if (StatsManager.instance.shotgunUnlocked) {
            validPowerups.Add(PowerUpType.ShotgunSpeed);
        } else {
            validPowerups.Add(PowerUpType.UnlockShotgun);
        }
        if (!StatsManager.instance.pistolFullAuto) {
            validPowerups.Add(PowerUpType.PistolFullAuto);
        }
        if (StatsManager.instance.pistolDmgLvl < StatsManager.POWER_UP_MAX_LVL) {
            validPowerups.Add(PowerUpType.PistolDmg);
        }
        if (StatsManager.instance.deliveryMoneyLvl < StatsManager.POWER_UP_MAX_LVL) {
            validPowerups.Add(PowerUpType.DeliveryMoney);
        }
        if (StatsManager.instance.runSpeedLvl < StatsManager.POWER_UP_MAX_LVL) {
            validPowerups.Add(PowerUpType.RunSpeed);
        }
        RandomExtensions.Shuffle(validPowerups);
        if (Player.instance.CurrentHealth < Player.instance.maxHealth) {
            validPowerups.Insert(0, PowerUpType.Heal);
        }
    }

    private BuyableItemInfo GetItem(PowerUpType powerUpType) {
        switch (powerUpType) {
            case PowerUpType.PistolDmg:
                return new BuyableItemInfo() {
                    title = "Increase Pistol Damage lvl " + (StatsManager.instance.pistolDmgLvl + 1),
                    price = 20 * (StatsManager.instance.pistolDmgLvl + 1),
                    onBuy = () => {
                        StatsManager.instance.pistolDmgLvl++;
                    }
                };
            case PowerUpType.PistolFullAuto:
                return new BuyableItemInfo() {
                    title = "Make Pistol Fully Automatic",
                    price = 50,
                    onBuy = () => {
                        StatsManager.instance.pistolFullAuto = true;
                    }
                };
            case PowerUpType.RunSpeed:
                return new BuyableItemInfo() {
                    title = "Increase Run Speed Lvl " + (StatsManager.instance.runSpeedLvl + 1),
                    price = 25 * (StatsManager.instance.runSpeedLvl + 1),
                    onBuy = () => {
                        StatsManager.instance.runSpeedLvl++;
                        Player.instance.SetMoveSpeed(StatsManager.instance.RunSpeed);
                    }
                };
            case PowerUpType.UnlockShotgun:
                return new BuyableItemInfo() {
                    title = "Unlock Shotgun" + Environment.NewLine + "(Press Q)",
                    price = 75,
                    onBuy = () => {
                        Player.instance.SetSwitchWeaponTextActive(true);
                        StatsManager.instance.shotgunUnlocked = true;
                    }
                };
            case PowerUpType.ShotgunSpeed:
                return new BuyableItemInfo() {
                    title = "Increase Shotgun Reload Speed lvl " + (StatsManager.instance.shotgunSpeedLvl + 1),
                    price = 20 * (StatsManager.instance.shotgunSpeedLvl + 1),
                    onBuy = () => {
                        StatsManager.instance.shotgunSpeedLvl++;
                    }
                };
            case PowerUpType.DeliveryMoney:
                return new BuyableItemInfo() {
                    title = "Receive more money for deliveries lvl " + (StatsManager.instance.deliveryMoneyLvl + 1),
                    price = 25 * (StatsManager.instance.deliveryMoneyLvl + 1),
                    onBuy = () => {
                        StatsManager.instance.deliveryMoneyLvl++;
                    }
                };
            case PowerUpType.Heal:
                int healthDiff = Player.instance.maxHealth - Player.instance.CurrentHealth;
                return new BuyableItemInfo() {
                    title = "Full Heal (" + healthDiff + " HP)",
                    price = healthDiff,
                    onBuy = () => {
                        Player.instance.FullHeal();
                    }
                };
            default:
                return new BuyableItemInfo() {
                    title = "Unknown PowerUp Type",
                    price = 0,
                    onBuy = () => { }
                };
        }
    }

    private void Clear() {
        for (int i = 0; i < currentElements.Count; i++) {
            BuyableElement ba = currentElements[i];
            if (ba != null && ba.gameObject != null) {
                Destroy(ba.gameObject);
            }
        }
        currentElements.Clear();
    }

    private void Continue() {
        AudioManager.Instance.PlayUIClick();
        AudioManager.Instance.FadeOutBGM();
        Vector2 startPos = bgTransform.anchoredPosition;
        this.CreateAnimationRoutine(1f, (float progress) => {
            bgTransform.anchoredPosition = Vector2.Lerp(startPos, NightStartUI.OFF_SCREEN_POS, Easing.easeInSine(0, 1, progress));
        }, onComplete);
    }

    private enum PowerUpType {
        PistolDmg,
        PistolFullAuto,
        RunSpeed,
        UnlockShotgun,
        ShotgunSpeed,
        DeliveryMoney,
        Heal
    }
}
