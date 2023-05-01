using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class JumpPad : MonoBehaviour {
    [Min(0)]
    public float jumpStrength;
    public AudioClip jumpSound;

    private float lastJumpTime;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            FirstPersonController fps = other.gameObject.GetComponent<FirstPersonController>();
            if(fps != null && Time.time - lastJumpTime > 0.2f) {
                lastJumpTime = Time.time;
                AudioManager.Instance.PlaySFX(jumpSound, 1f);
                fps.SuperJump(jumpStrength);
            }
        }
    }
}
