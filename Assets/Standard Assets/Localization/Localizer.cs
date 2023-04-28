using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Localizer {
    public static event LanguageChangedDelegate LanguageChangedEvent;
    public delegate void LanguageChangedDelegate();
    private static Dictionary<string, Dictionary<string, string>> languages;
    private static Dictionary<string, string> currentLanguage;
    public static string currentLanguageName;
    public static bool languageLoaded = false;
    public const string ENGLISH_KEY = "english";
    public const string SPANISH_KEY = "spanish";
    public const string JAPANESE_KEY = "japanese";

    public static void EnsureLoaded() {
        if (languages == null) {
            LoadLanguageFiles();
            LoadLanguage(ENGLISH_KEY);
        }
    }
    private const int ID_COLUMN = 0;
    private const int ENGLISH_COLUMN = 1;
    private const int SPANISH_COLUMN = 2;
    //public static readonly string LOC_FILE_PATH = Application.streamingAssetsPath + "/localization.csv";
    public static void LoadLanguageFiles() {
        Dictionary<string, string> englishDict = new Dictionary<string, string>();
        Dictionary<string, string> spanishDict = new Dictionary<string, string>();

        //string csvString = System.IO.File.ReadAllText(LOC_FILE_PATH, System.Text.Encoding.UTF8);

        string csvString = Resources.Load<TextAsset>("localization").text;
        List<List<string>> csvGrid = CSVParser.LoadFromString(csvString);
        for (int i = 0; i < csvGrid.Count; i++) {
            List<string> row = csvGrid[i];
            englishDict[row[ID_COLUMN]] = DoReplacements(row[ENGLISH_COLUMN]);
            spanishDict[row[ID_COLUMN]] = DoReplacements(row[SPANISH_COLUMN]);
        }

        languages = new Dictionary<string, Dictionary<string, string>> {
            { ENGLISH_KEY , englishDict },
            { SPANISH_KEY , spanishDict },
        };

        languageLoaded = true;
    }

    private static string DoReplacements(string inputString) {
        return inputString.Replace("\\n", Environment.NewLine).Replace('‘', '\'').Replace('’', '\'');
    }

    public static void LoadLanguage(string languageName) {
        if (languageName != currentLanguageName) {
            currentLanguageName = languageName;
            currentLanguage = languages[languageName];
            LanguageChangedEvent?.Invoke();
        }
    }

    public static string GetText(string key) {
        if (!languageLoaded)
            EnsureLoaded();

        if (currentLanguage.TryGetValue(key, out string result)) {
            return result;
        } else {
            Debug.LogError("Key not found in language dictionary: " + key);
            return "";
        }
    }

    public static Dictionary<string, string> GetCurrentLanguage() {
        EnsureLoaded();
        return languages[currentLanguageName];
    }

    public static TMP_FontAsset CurrentLangDefaultFont {
        get {
            EnsureLoaded();
            switch (currentLanguageName) {
                case ENGLISH_KEY:
                    return GetFont("MainMenu_Font");
                case JAPANESE_KEY:
                    return GetFont("PixelMplus12-Regular SDF");
            }
            return null;
        }
    }

    public static bool CurrentLangageInYarnFiles() {
        return currentLanguageName == ENGLISH_KEY;
    }


    private static readonly Dictionary<string, TMP_FontAsset> fontDictionary = new Dictionary<string, TMP_FontAsset>();
    private static TMP_FontAsset GetFont(string fontName) {
        if (fontDictionary.TryGetValue(fontName, out TMP_FontAsset result)) {
            return result;
        } else {
            result = (TMP_FontAsset)ResourceManager.LoadResource("Fonts/" + fontName);
            fontDictionary.Add(fontName, result);
        }
        return result;
    }
}
