using UnityEngine;

public class Destination : MonoBehaviour {
    public Transform mainT;
    public Transform minimapIconT;
    [SerializeField] private GameObject destinationSphere;

    private void Start() {
        minimapIconT.eulerAngles = new Vector3(270, 180, 0);
        DeliveryManager.instance.RegisterDestination(this);
    }

    private void OnDestroy() {
        if (DeliveryManager.instance != null) {
            DeliveryManager.instance.UnregisterDestination(this);
        }
    }

    public void SetDestinationSelected(bool selected) {
        destinationSphere.SetActive(selected);
    }
}
