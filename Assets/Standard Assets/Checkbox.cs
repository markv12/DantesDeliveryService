using System;
using UnityEngine;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour {
    [SerializeField] Button mainButton;
    [SerializeField] Image mainImage;
    [SerializeField] Sprite uncheckedSprite;
    [SerializeField] Sprite checkedSprite;

    public event Action<bool> onChange;

    private bool isChecked;
    public bool IsChecked {
        get {
            return isChecked;
        }
        set {
            isChecked = value;
            mainImage.sprite = isChecked ? checkedSprite : uncheckedSprite;
            onChange?.Invoke(isChecked);
        }
    }

    private void Awake() {
        mainButton.onClick.AddListener(() => { IsChecked = !IsChecked; });
    }
}
