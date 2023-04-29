using UnityEngine;

public class DeliveryObject : MonoBehaviour {
    public Transform mainT;
    public Rigidbody mainRigidbody;
    public Transform arrowT;
    public SpriteRenderer arrowRenderer;

    public Destination destination;

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Destination")) {
            Destination hitDestination = other.gameObject.GetComponent<Destination>();
            if (hitDestination != null && destination == hitDestination) {
                Player.instance.DOHitDestination(this);
                Destroy(gameObject);
            }
        }
    }

    private void Update() {
        if(destination != null) {
            Vector3 posDiff = (mainT.position - destination.mainT.position).SetY(0);
            float length = (posDiff.magnitude/2f) - 1f;
            Vector3 midPoint = ((mainT.position + destination.mainT.position) / 2f).SetY(10);
            arrowT.position = midPoint;

            float angle = -AngleUtil.CartesianToAngle(new Vector2(posDiff.x, posDiff.z));
            arrowT.eulerAngles = new Vector3(0, angle, 0);
            arrowRenderer.size = new Vector2(length, 4);
        }
    }
}
