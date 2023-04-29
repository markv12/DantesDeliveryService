using UnityEngine;
using UnityEngine.AI;

public class GroundEnemy : MonoBehaviour {
    public NavMeshAgent navMeshAgent;

    private void Update() {
        if(Player.instance != null) {
            navMeshAgent.destination = Player.instance.transform.position;
        }
    }
}
