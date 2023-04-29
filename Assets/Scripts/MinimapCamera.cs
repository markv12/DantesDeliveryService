using UnityEngine;

public class MinimapCamera : MonoBehaviour {
    public Transform cameraT;
    public Camera mainCamera;

    private bool followPlayerMode = true;
    public bool FollowPlayerMode {
        get {
            return followPlayerMode;
        }
        set {
            followPlayerMode = value;
            if (followPlayerMode) {
                mainCamera.orthographicSize = 50;
            } else {
                cameraT.position = new Vector3(0, 300, 0);
                mainCamera.orthographicSize = 100;
            }
        }
    }
    private void Update() {
        if (followPlayerMode && Player.instance != null) {
            Vector3 playerPos = Player.instance.transform.position;
            cameraT.position = new Vector3(playerPos.x, cameraT.position.y, playerPos.z);
        }
    }
}
