using System.Collections.Generic;
using UnityEngine;

public static class WaitUtil {
    private static readonly Dictionary<float, WaitForSeconds> waits = new Dictionary<float, WaitForSeconds>(16);
    public static WaitForSeconds GetWait(float length) {
        if(!waits.TryGetValue(length, out WaitForSeconds wait)) {
            wait = new WaitForSeconds(length);
            waits.Add(length, wait);
        }
        return wait;
    }
}
