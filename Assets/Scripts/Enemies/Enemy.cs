using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    [Min(0)]
    public int maxHealth;
    public int touchDamage;
    public float projectileRange;
    public Transform spriteT;
    public SpriteRenderer mainRenderer;

    public Sprite normalSprite;
    public Sprite attackSprite;
    public Sprite deathSprite;

    public AudioClip deathSound;
    public AudioClip chargeSound;
    public AudioClip fireSound;
    public Projectile projectile;
    private float lastProjectileTime;
    public float timeBetweenProjectiles;

    public NavMeshAgent navMeshAgent;
    private Vector3 startScale;

    private int health;
    private void Awake() {
        health = maxHealth;
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
                if (playerSqrDist < 3f) {
                    Player.instance.Hurt(touchDamage);
                    Destroy(gameObject);
                    isDestroyed = true;
                } else if (playerSqrDist < (projectileRange * projectileRange)) {
                    if (Time.time - lastProjectileTime > timeBetweenProjectiles) {
                        lastProjectileTime = Time.time;
                        FireProjectile();
                    }
                }
            }
            if (!DayNightManager.instance.IsNight) {
                Destroy(gameObject);
                isDestroyed = true;
            }
        }
    }

    private Coroutine fireProjectileRoutine;
    private void FireProjectile() {
        this.EnsureCoroutineStopped(ref fireProjectileRoutine);
        fireProjectileRoutine = StartCoroutine(FireProjectileRoutine());

        IEnumerator FireProjectileRoutine() {
            mainRenderer.sprite = attackSprite;
            AudioManager.Instance.PlaySFX(chargeSound, 1f);
            yield return WaitUtil.GetWait(0.4f);
            mainRenderer.sprite = normalSprite;
            AudioManager.Instance.PlaySFX(fireSound, 1f);
            if (Player.instance != null && navMeshAgent.enabled) {
                Vector3 playerPos = Player.instance.transform.position.AddY(1f);
                Vector3 playerDiff = playerPos - spriteT.position;
                Projectile newProjectile = Instantiate(projectile);
                Vector3 startPos = spriteT.position;
                newProjectile.mainT.SetPositionAndRotation(startPos, spriteT.rotation);
                Vector3 playerDir = playerDiff.normalized;
                Vector3 endPos = startPos + playerDir * 50;
                newProjectile.Execute(startPos, endPos);
            }
        }
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
        this.EnsureCoroutineStopped(ref fireProjectileRoutine);
        navMeshAgent.enabled = false;
        mainRenderer.sprite = deathSprite;
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
