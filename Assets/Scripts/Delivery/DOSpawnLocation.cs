using UnityEngine;

public class DOSpawnLocation : MonoBehaviour {
    public Transform mainT;

    private void Start() {
        DeliveryManager.instance.RegisterSpawnLocation(this);
    }

    private void OnDestroy() {
        if (DeliveryManager.instance != null) {
            DeliveryManager.instance.UnregisterSpawnLocation(this);
        }
    }
}
