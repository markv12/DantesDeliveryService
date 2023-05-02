using UnityEngine;

public class NPC : MonoBehaviour {
    public Transform mainT;
    void Start() {
        DayNightManager.instance.IsNightChanged += HandleIsNightChanged;
    }

    private void HandleIsNightChanged(bool isNight) {
        gameObject.SetActive(!isNight);
    }

    // Update is called once per frame
    void Update() {
        if (Player.instance != null) {
            Vector3 targetPos = Player.instance.transform.position.SetY(mainT.position.y);
            mainT.LookAt(targetPos);
        }
    }
}
