using UnityEngine;

public class BobUpAndDown : MonoBehaviour {
    // User Inputs
    public float amplitude = 0.1f;
    public float frequency = 0.3f;
    private float randomOffset = 0;

    // Position Storage Variables
    float startY;

    void Start() {
        // Store the starting position & rotation of the object
        startY = transform.localPosition.y;
        randomOffset = Random.Range(0f, 5f);
    }

    void Update() {
        // Float up/down with a Sin()
        float y = startY + Mathf.Sin(Time.fixedTime * Mathf.PI * frequency + randomOffset) * amplitude;
        transform.localPosition = transform.localPosition.SetY(y);
    }
}
