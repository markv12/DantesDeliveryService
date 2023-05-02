using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ExplodingEnemy : MonoBehaviour {
    [Min(0)]
    public int maxHealth;
    public int explodeDamage;
    [Min(0)]
    public int moneyOnDeath;
    public Transform spriteT;
    public SpriteRenderer mainRenderer;

    public Sprite normalSprite;
    public Sprite lvl1Sprite;
    public Sprite lvl2Sprite;
    public Sprite explodeSprite;

    public AudioClip explodeSound;
    public AudioClip deathSound;

    public AudioSource loopSound;

    public NavMeshAgent navMeshAgent;
    private Vector3 startScale;

    private int health;
    private void Awake() {
        health = maxHealth;
        loopSound.Play();
        startScale = spriteT.localScale;
    }

    bool isDestroyed = false;
    private void Update() {
        if (!isDestroyed) {
            if (Player.instance != null && navMeshAgent.enabled) {
                Vector3 playerPos = Player.instance.transform.position;
                navMeshAgent.destination = playerPos;
                Vector3 playerDiff = playerPos - spriteT.position;
                float playerSqrDist = playerDiff.sqrMagnitude;
                if (playerSqrDist < 8f) {
                    Explode();
                    isDestroyed = true;
                } else if(playerSqrDist < 36) {
                    mainRenderer.sprite = lvl2Sprite;
                    loopSound.pitch = 2f;
                } else if (playerSqrDist < 144) {
                    mainRenderer.sprite = lvl1Sprite;
                    loopSound.pitch = 1.5f;
                } else {
                    mainRenderer.sprite = normalSprite;
                    loopSound.pitch = 1f;
                }
            }
            if (!DayNightManager.instance.IsNight) {
                Destroy(gameObject);
                isDestroyed = true;
            }
        }
    }

    private void Explode() {
        loopSound.Stop();
        mainRenderer.sprite = explodeSprite;
        Player.instance.Hurt(explodeDamage);
        Die();
    }

    private bool died = false;
    public void Hurt(int amount) {
        if (!died) {
            health -= amount;
            FlashColor(Color.red);
            if (health <= 0) {
                Die();
            }
        }
    }

    private void Die() {
        died = true;
        StatsManager.instance.AddMoney(moneyOnDeath);
        navMeshAgent.enabled = false;
        mainRenderer.sprite = normalSprite;
        AudioManager.Instance.PlaySFX(deathSound, 1f);
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
            Vector3 flashScale = startScale *= 0.92f;
            mainRenderer.color = color;
            spriteT.localScale = flashScale;
            yield return flashWait;
            mainRenderer.color = Color.white;
            spriteT.localScale = startScale;
        }
    }
}
