#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WorldSpriteLocalizer : MonoBehaviour {
    public SpriteRenderer mainRenderer;
    void Awake() {
        RefreshSprite();
        Localizer.LanguageChangedEvent += RefreshSprite;
    }

    void RefreshSprite() {
        mainRenderer.sprite = SpriteLocalizer.GetLocalizedSprite(mainRenderer.sprite);
    }

    void OnDestroy() {
        Localizer.LanguageChangedEvent -= RefreshSprite;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(WorldSpriteLocalizer))]
public class WorldSpriteLocalizerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        WorldSpriteLocalizer model = (WorldSpriteLocalizer)target;
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("Get Component", GUILayout.Width(EditorGUIUtility.currentViewWidth * .45f))) {
            model.mainRenderer = model.GetComponent<SpriteRenderer>();
        }
        GUILayout.EndHorizontal();
    }
}
#endif
