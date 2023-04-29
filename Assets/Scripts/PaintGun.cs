using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PaintGun : MonoBehaviour {
    public RectTransform rectT;
    public Transform raycastT;
    public Image smokeImage;
    public Image colorIndicator;
    public LayerMask layerMask;
    public Sprite[] splatSprites;
    public AudioClip shootSound;
    public AudioClip equipSound;
    public AudioClip unequipSound;

    private bool singleColorMode;
    private Color selectedColor;

    private static int paintingLayer;
    private void Awake() {
        paintingLayer = LayerMask.NameToLayer("Default");
    }

    private const float RAYCAST_DISTANCE = 50f;
    public void Shoot() {
        AudioManager.Instance.PlaySFX(shootSound, 1, Random.Range(0.90f, 1.1f));

        Kickback();
        if (Physics.Raycast(raycastT.position, raycastT.forward, out RaycastHit hit, RAYCAST_DISTANCE, layerMask)) {
            CreateSplat(hit);
        }
    }

    private const float AUTO_WAIT = 0.25f;
    private float lastShootTime;
    private const float TIME_BETWEEN_SHOTS = 0.08f;
    private float timeHeld = 0;
    public void Hold() {
        timeHeld += Time.deltaTime;
        if(timeHeld > AUTO_WAIT) {
            if(Time.time - lastShootTime > TIME_BETWEEN_SHOTS) {
                lastShootTime = Time.time;
                Shoot();
            }
        }
    }

    public void EndHold() {
        timeHeld = 0;
    }

    private Coroutine kickbackRoutine;
    private Coroutine kickbackSubroutine;
    private const float KICK_TIME = 0.2f;
    private const float RETURN_TIME = 0.3f;
    private void Kickback() {
        StopKick();
        smokeImage.gameObject.SetActive(false);
        kickbackRoutine = StartCoroutine(KickbackRoutine());

        IEnumerator KickbackRoutine() {
            Vector2 startPos = rectT.anchoredPosition;
            Vector2 dirToEquip = equippedPos - startPos;
            Vector2 endPos = startPos + shootVector + (dirToEquip/5f);
            float startTime = Time.time;
            kickbackSubroutine = this.CreateAnimationRoutine(KICK_TIME, (float progress) => {
                rectT.anchoredPosition = Vector2.Lerp(startPos, endPos, Easing.easeOutQuad(0, 1, progress));
            });
            smokeImage.gameObject.SetActive(true);
            yield return null;
            yield return null;
            smokeImage.gameObject.SetActive(false);
            while (Time.time - startTime < KICK_TIME) {
                yield return null;
            }

            startPos = rectT.anchoredPosition;
            kickbackSubroutine = this.CreateAnimationRoutine(RETURN_TIME, (float progress) => {
                rectT.anchoredPosition = Vector2.Lerp(startPos, equippedPos, Easing.easeOutSine(0, 1, progress));
            });
        }
    }

    private void StopKick() {
        this.EnsureCoroutineStopped(ref kickbackRoutine);
        this.EnsureCoroutineStopped(ref kickbackSubroutine);
    }

    private void CreateSplat(RaycastHit hit) {
        GameObject newSplat = new GameObject("Splat");
        Vector3 pos = hit.point + (hit.normal * 0.02f);
        newSplat.transform.position = pos;
        newSplat.transform.rotation = Quaternion.LookRotation(hit.normal);
        newSplat.transform.RotateAround(pos, newSplat.transform.forward, Random.Range(0f, 360f));
        newSplat.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        SpriteRenderer splatRenderer = newSplat.AddComponent<SpriteRenderer>();
        splatRenderer.sprite = splatSprites[Random.Range(0, splatSprites.Length)];
        splatRenderer.sortingOrder = 1;
        newSplat.layer = paintingLayer;
    }

    private Coroutine equipRoutine;
    private static readonly Vector2 equippedPos = new Vector2(300f, -277f);
    private static readonly Vector2 unequippedPos = new Vector2(300f, -807);
    private const float SHOOT_MAGNITUDE = 30;
    private static readonly Vector2 shootVector = new Vector2(0.5f, -0.866f) * SHOOT_MAGNITUDE;
    public void SetEquipped(bool equipped) {
        StopKick();
        if(equipped) {
            gameObject.SetActive(true);
        }
        AudioManager.Instance.PlaySFX(equipped ? equipSound : unequipSound, 1);

        this.EnsureCoroutineStopped(ref equipRoutine);
        Vector2 startPos = rectT.anchoredPosition;
        Vector2 endPos = equipped ? equippedPos : unequippedPos; 
        equipRoutine = this.CreateAnimationRoutine(1f, (float progress) => {
            rectT.anchoredPosition = Vector2.Lerp(startPos, endPos, Easing.easeInOutSine(0, 1, progress));
        }, () => {
            if(!equipped) {
                gameObject.SetActive(false);
            }
        });
    }
}
