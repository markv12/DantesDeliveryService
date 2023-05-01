using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions {
    public static void Shuffle<T>(T[] array) {
        int n = array.Length;
        while (n > 1) {
            int k = Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

    public static void Shuffle<T>(List<T> list) {
        int n = list.Count;
        while (n > 1) {
            int k = Random.Range(0, n--);
            T temp = list[n];
            list[n] = list[k];
            list[k] = temp;
        }
    }
}
