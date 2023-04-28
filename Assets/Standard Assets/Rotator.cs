using UnityEngine;

public class Rotator : MonoBehaviour {
    public Transform mainTransform;
    public Vector3 rotation;

    void Update() {
        mainTransform.Rotate(rotation * Time.deltaTime);
    }
}
