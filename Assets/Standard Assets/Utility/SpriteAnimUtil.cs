using System.Collections;
using UnityEngine;

public class SpriteAnimUtil {
    public static IEnumerator SpriteAnimRoutine(SpriteRenderer renderer, Sprite[] sprites, int fps, bool disableOnComplete = true) {
        float timePerFrame = 1f / fps;

        for (int i = 0; i < sprites.Length; i++) {
            renderer.sprite = sprites[i];
            float elapsedTime = 0;
            while (elapsedTime < timePerFrame) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        if (disableOnComplete) {
            renderer.enabled = false;
        }
    }

    public static IEnumerator SpriteLoopRoutine(SpriteRenderer renderer, Sprite[] sprites, int fps) {
        int currentSpriteIndex = Random.Range(0, sprites.Length);
        float timePerFrame = 1f / fps;

        while (true) {
            renderer.sprite = sprites[currentSpriteIndex];
            float elapsedTime = 0;
            while (elapsedTime < timePerFrame) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currentSpriteIndex++;
            currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;
        }
    }
}
