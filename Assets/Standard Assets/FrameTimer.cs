using System.Diagnostics;
using UnityEngine;

public class FrameTimer : MonoBehaviour {
    public static FrameTimer instance;

    private Stopwatch stopwatch;
    private const float MILLISECOND_BUDGET = 7f;
    private static long tickBudget;
    public static bool AtEndOfFrame => instance.stopwatch.ElapsedTicks > tickBudget;

    void Awake() {
        tickBudget = (long)(Stopwatch.Frequency * MILLISECOND_BUDGET / 1000f);
        instance = this;
        stopwatch = new Stopwatch();
    }
    void Update() {
        // For whatever reason, .Restart() wasn't recognized.
        stopwatch.Reset();
        stopwatch.Start();
    }
}
