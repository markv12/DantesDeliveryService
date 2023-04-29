using System.Collections.Generic;
using UnityEngine;

public static class PauseManager {
    private static readonly List<MonoBehaviour> requesters = new List<MonoBehaviour>(4);

    public static void RequestPause(MonoBehaviour requester) {
        requesters.Add(requester);
        Time.timeScale = requesters.Count > 0 ? 0 : 1;
    }

    public static void ReleasePause(MonoBehaviour requester) {
        requesters.Remove(requester);
        Time.timeScale = requesters.Count > 0 ? 0 : 1;
    }
}
