using UnityEngine;

public class DeliveryObject : MonoBehaviour {
    public Transform mainT;
    public Rigidbody mainRigidbody;
    public Destination destination;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Destination")) {
            Destination hitDestination = other.gameObject.GetComponent<Destination>();
            if (hitDestination != null && destination == hitDestination) {
                Destroy(gameObject);
            }
        }
    }
}
