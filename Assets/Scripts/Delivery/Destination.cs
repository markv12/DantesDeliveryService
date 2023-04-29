using UnityEngine;

public class Destination : MonoBehaviour {
    public Transform mainT;

    private void Start() {
        DeliveryManager.instance.RegisterDestination(this);
    }

    private void OnDestroy() {
        if (DeliveryManager.instance != null) {
            DeliveryManager.instance.UnregisterDestination(this);
        }
    }
}
