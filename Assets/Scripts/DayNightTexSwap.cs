using UnityEngine;

public class DayNightTexSwap : MonoBehaviour {
    public MeshRenderer mainRenderer;
    public Texture2D dayTex;
    public Texture2D nightTex;
    public Vector2 textureScale;

    private void Start() {
        DayNightManager.instance.IsNightChanged += HandleIsNightChanged;
    }

    private void HandleIsNightChanged(bool isNight) {
        mainRenderer.material.SetTexture("_MainTex", isNight ? nightTex : dayTex);
        mainRenderer.material.SetTextureScale("_MainTex", textureScale);
    }
}
