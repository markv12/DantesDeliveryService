using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    [Min(0)]
    public int maxHealth;
    public Transform spriteT;
    public SpriteRenderer mainRenderer;

    public Sprite normalSprite;
    public Sprite attackSprite;
    public Sprite deathSprite;

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
            Die();
        }
    }

    private void Die() {
        navMeshAgent.enabled = false;
        mainRenderer.sprite = deathSprite;
        Vector3 startScale = spriteT.localScale;
        Vector3 endScale = Vector3.zero;
        this.CreateAnimationRoutine(0.8f, (float progress) => {
            spriteT.localScale = Vector3.Lerp(startScale, endScale, Easing.easeInSine(0, 1, progress));
        }, () => {
            Destroy(gameObject);
        });
    }

    private const float FLASH_DURATION = 0.05f;
    private static readonly WaitForSeconds flashWait = new WaitForSeconds(FLASH_DURATION);
    private Coroutine flashRoutine;
    private void FlashColor(Color color) {
        this.EnsureCoroutineStopped(ref flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
        IEnumerator FlashRoutine() {
            Vector3 startScale = spriteT.localScale;
            Vector3 flashScale = startScale *= 0.92f;
            mainRenderer.color = color;
            spriteT.localScale = flashScale;
            yield return flashWait;
            mainRenderer.color = Color.white;
            spriteT.localScale = startScale;
        }
    }
}
