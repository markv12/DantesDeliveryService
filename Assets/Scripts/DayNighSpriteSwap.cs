using UnityEngine;

public class DayNightSpriteSwap : MonoBehaviour {
    public SpriteRenderer mainRenderer;
    public Sprite daySprite;
    public Sprite nightSprite;

    private void Start() {
        DayNightManager.instance.Register(this);
    }

    private void OnDestroy() {
        if(DayNightManager.instance != null) {
            DayNightManager.instance.Unregister(this);
        }
    }
}
