using System;
using UnityEngine;

public class DeliveryObject : MonoBehaviour {
    public Transform mainT;
    public Rigidbody mainRigidbody;
    public Transform minimapArrowT;
    public GameObject pickupArrow;
    public Transform minimapIconT;
    public SpriteRenderer arrowRenderer;

    [NonSerialized] public DOSpawnLocation spawnLocation;
    [NonSerialized] public Destination destination;

    private void Awake() {
        minimapIconT.eulerAngles = new Vector3(270, 180, 0);
        minimapArrowT.SetParent(null, true);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Destination")) {
            Destination hitDestination = other.gameObject.GetComponent<Destination>();
            if (hitDestination != null && destination == hitDestination) {
                hitDestination.SetDestinationSelected(false);
                Player.instance.DOHitDestination(this);
                Destroy(gameObject);
                DeliveryManager.instance.SpawnNewDelivery();
            }
        }
    }

    private void Update() {
        if(destination != null) {
            Vector3 posDiff = (mainT.position - destination.mainT.position).SetY(0);
            float length = posDiff.magnitude - 1.5f;
            Vector3 midPoint = ((mainT.position + destination.mainT.position) / 2f).SetY(100);
            minimapArrowT.position = midPoint;

            float angle = -AngleUtil.CartesianToAngle(new Vector2(posDiff.x, posDiff.z));
            minimapArrowT.eulerAngles = new Vector3(0, angle, 0);
            arrowRenderer.size = new Vector2(5f, length);
        }
    }

    public void RemoveFromSpawnLocation() {
        if (spawnLocation != null) {
            spawnLocation.currentDO = null;
            spawnLocation = null;
        }
        if(pickupArrow != null) {
            Destroy(pickupArrow);
        }
    }

    private void OnDestroy() {
        if(minimapArrowT != null) {
            Destroy(minimapArrowT.gameObject);
        }
    }
}
