using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SpeedUpPad : MonoBehaviour {
    [Min(1)]
    public float speedModifier;
    [Min(0)]
    public float duration;
    private float lastSpeedUpTime;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            FirstPersonController fps = other.gameObject.GetComponent<FirstPersonController>();
            if(fps != null && Time.time - lastSpeedUpTime > 0.2f && Vector3.Dot(fps.CurrentDirection.normalized, transform.forward) > 0.1f) {
                lastSpeedUpTime = Time.time;
                fps.SpeedUp(speedModifier, duration);
            }
        }
    }
}
