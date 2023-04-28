using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteLocalizer", menuName = "Sprite Localizer")]
[PreferBinarySerialization]
public class SpriteLocalizer : ScriptableObject {
    [SerializeField] private SpriteSet[] spriteSets;
    private Dictionary<string, Dictionary<string, Sprite>> languages;

    private static SpriteLocalizer instance;
    public static SpriteLocalizer Instance {
        get {
            if (instance == null) {
                instance = Resources.Load<SpriteLocalizer>("SpriteLocalizer");
                instance.SetupLanguages();
            }
            return instance;
        }
    }

    private void SetupLanguages() {
        Dictionary<string, Sprite> englishSprites = new Dictionary<string, Sprite>();
        Dictionary<string, Sprite> spanishSprites = new Dictionary<string, Sprite>();
        for (int i = 0; i < spriteSets.Length; i++) {
            SpriteSet spriteSet = spriteSets[i];
            Sprite englishSprite = spriteSet.englishSprite;
            string spriteName = englishSprite.name;

            englishSprites[spriteName] = englishSprite;
            spanishSprites[spriteName] = (spriteSet.spanishSprite == null) ? englishSprite : spriteSet.spanishSprite;
        }
        languages = new Dictionary<string, Dictionary<string, Sprite>> {
            { Localizer.ENGLISH_KEY , englishSprites },
            { Localizer.SPANISH_KEY , spanishSprites },
        };
    }

    public static Sprite GetLocalizedSprite(Sprite inputSprite) {
        Localizer.EnsureLoaded();
        Dictionary<string, Sprite> currentLang = Instance.languages[Localizer.currentLanguageName];
        if (currentLang == null) { return inputSprite; }
        Sprite localizedSprite = currentLang[inputSprite.name];
        return (localizedSprite == null) ? inputSprite : localizedSprite;
    }

    [Serializable]
    public class SpriteSet {
        public Sprite englishSprite;
        public Sprite spanishSprite;
    }
}
