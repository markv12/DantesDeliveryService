using UnityEngine;
using UnityEngine.AI;

public class GroundEnemy : MonoBehaviour {
    [Min(0)]
    public int maxHealth;
    public NavMeshAgent navMeshAgent;

    private int health;
    private void Awake() {
        health = maxHealth;
    }

    private void Update() {
        if (Player.instance != null) {
            navMeshAgent.destination = Player.instance.transform.position;
        }
    }

    public void Hurt(int amount) {
        health -= amount;
        if(health <= 0) {
            Destroy(gameObject);
        }
    }
}
