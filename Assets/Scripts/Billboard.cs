using UnityEngine;

public class Billboard : MonoBehaviour {
    public Transform billboardT;

    void Update() {
        if (Player.instance != null) {
            Vector3 targetPos = Player.instance.t.position.SetY(billboardT.position.y);
            billboardT.LookAt(targetPos);
        }
    }
}