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
        for (int i = 0; i < 10; i++) {
            int x = i % 2;
            int y = i / 2;
            BuyableElement newElement = Instantiate(buyableElementPrefab, elementContainer);
            newElement.rectT.anchoredPosition = new Vector2(x * 350, y * -120);
            newElement.Setup(GetItem());
            currentElements.Add(newElement);
        }
    }

    private BuyableItemInfo GetItem() {
        return new BuyableItemInfo() {
            title = "Derp",
            price = UnityEngine.Random.Range(1, 10),
            onBuy = () => {
                Debug.Log("bought!");
            }
        };
    }

    private void Clear() {
        for (int i = 0; i < currentElements.Count; i++) {
            Destroy(currentElements[i].gameObject);
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
}
