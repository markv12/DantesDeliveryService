using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SpeedUpPad : MonoBehaviour {
    [Min(1)]
    public float speedModifier;
    [Min(0)]
    public float duration;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            FirstPersonController fps = other.gameObject.GetComponent<FirstPersonController>();
            if(fps != null && Vector3.Dot(fps.CurrentDirection.normalized, transform.forward) > 0.1f) {
                fps.SpeedUp(speedModifier, duration);
            }
        }
    }
}
