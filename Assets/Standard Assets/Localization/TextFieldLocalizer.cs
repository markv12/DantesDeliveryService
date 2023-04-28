using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TextFieldLocalizer : MonoBehaviour {
    public TMP_Text textField;
    public string locKey;
    void Awake() {
        if(string.IsNullOrWhiteSpace(locKey)) {
            locKey = textField.text;
        }
        RefreshText();
        Localizer.LanguageChangedEvent += RefreshText;
    }

    void RefreshText() {
        //textField.font = Localizer.CurrentLangDefaultFont;
        textField.text = Localizer.GetText(locKey);
    }

    void OnDestroy() {
        Localizer.LanguageChangedEvent -= RefreshText;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextFieldLocalizer))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
public class TextFieldLocalizerViewer : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        TextFieldLocalizer model = (TextFieldLocalizer)target;
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("Get Component", GUILayout.Width(EditorGUIUtility.currentViewWidth * .45f))) {
            model.textField = model.GetComponent<TMP_Text>();
        }
        GUILayout.EndHorizontal();
    }
}
#endif
