using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class JumpPad : MonoBehaviour {
    [Min(0)]
    public float jumpStrength;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            FirstPersonController fps = other.gameObject.GetComponent<FirstPersonController>();
            if(fps != null) {
                fps.SuperJump(jumpStrength);
            }
        }
    }
}
