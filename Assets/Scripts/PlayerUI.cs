using System;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    [SerializeField] private RectTransform throwMeter;

    public void ShowThrowStrength(float throwStrength) {
        throwMeter.sizeDelta = new Vector2(60, 300f * throwStrength);
    }
}
