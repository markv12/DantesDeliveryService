using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyableElement : MonoBehaviour {
    public RectTransform rectT;
    public TMP_Text titleText;
    public TMP_Text priceText;
    public Button buyButton;

    private BuyableItemInfo? info;
    void Awake() {
        buyButton.onClick.AddListener(Buy);
    }

    public void Setup(BuyableItemInfo _info) {
        info = _info;
        titleText.text = info.Value.title;
        priceText.text = "$" + info.Value.price.ToString();
    }

    private void Buy() {
        info?.onBuy?.Invoke();
        Destroy(gameObject);
    }
}

public struct BuyableItemInfo {
    public string title;
    public int price;
    public Action onBuy;
}
