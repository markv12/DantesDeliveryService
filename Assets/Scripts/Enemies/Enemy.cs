using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    [Min(0)]
    public int maxHealth;
    public SpriteRenderer mainRenderer;
    public NavMeshAgent navMeshAgent;

    private int health;
    private void Awake() {
        health = maxHealth;
    }

    bool isDestroyed = false;
    private void Update() {
        if (Player.instance != null) {
            navMeshAgent.destination = Player.instance.transform.position;
        }
        if (!DayNightManager.instance.IsNight && !isDestroyed) {
            Destroy(gameObject);
            isDestroyed = true;
        }
    }

    public void Hurt(int amount) {
        health -= amount;
        FlashColor(Color.red);
        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    private const float FLASH_DURATION = 0.05f;
    private static readonly WaitForSeconds flashWait = new WaitForSeconds(FLASH_DURATION);
    private Coroutine flashRoutine;
    private void FlashColor(Color color) {
        this.EnsureCoroutineStopped(ref flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
        IEnumerator FlashRoutine() {
            mainRenderer.color = color;
            yield return flashWait;
            mainRenderer.color = Color.white;
        }
    }
}
