using System;
using UnityEngine;

public class DOSpawnLocation : MonoBehaviour {
    public Transform mainT;
    [NonSerialized] public DeliveryObject currentDO;

    private void Start() {
        DeliveryManager.instance.RegisterSpawnLocation(this);
    }

    private void OnDestroy() {
        if (DeliveryManager.instance != null) {
            DeliveryManager.instance.UnregisterSpawnLocation(this);
        }
    }
}
