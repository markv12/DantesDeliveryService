using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {
    public static Player instance;

    public Transform t;
    public Camera mainCamera;
    private Transform mainCameraTransform;

    public CharacterController characterController;
    public FirstPersonController firstPersonController;

    public PlayerUI playerUI;
    public Transform directionArrow;
    public Gun gun;
    public float maxHealth;
    private float health;

    public AudioClip pickUpSound;
    public AudioClip throwSound;
    public GameOverUI gameOverUI;

    private DeliveryObject currentDO;
    private DeliveryObject CurrentDO {
        get {
            return currentDO;
        }
        set {
            if(currentDO != value) {
                currentDO = value;
                DeliveryManager.instance.SetActiveDO(currentDO);
                if(currentDO == null && DayNightManager.instance.IsNight) {
                    GunEquipped = true;
                }
            }

        }
    }

    public Vector3 FaceDirection => mainCameraTransform.forward;

    private Vector3 playerStartPos;
    private Quaternion playerStartRotation;

    private void Awake() {
        instance = this;
        mainCameraTransform = mainCamera.transform;
        playerStartPos = t.position;
        playerStartRotation = t.rotation;
        directionArrow.gameObject.SetActive(false);
        health = maxHealth;
    }

    public void Hurt(int damage) {
        health = Mathf.Max(0, health - damage);
        playerUI.ShowHealthFraction(health / (float)maxHealth);
        if(health > 0) {
            AudioManager.Instance.PlayPlayerHurt();
        } else {
            if (!died) {
                Die();
            }
        }
    }

    private bool died = false;
    private void Die() {
        died = true;
        AudioManager.Instance.PlayPlayerDie();
        AudioManager.Instance.FadeOutBGM();
        gameOverUI.Show(() => {
            SetFPSControllerActive(false);
        });
    }

    private float throwStrength = 0;
    private float ThrowStrength {
        get {
            return throwStrength;
        }
        set {
            throwStrength = value;
            playerUI.ShowThrowStrength(EasedThrowStrength);
        }
    }
    private float EasedThrowStrength => Easing.easeOutQuad(0, 1, throwStrength);

    private float lastThrowTime;
    private void Update() {
        if (GunEquipped) {
            if (InputUtil.LeftMouseButtonDown) {
                gun.Shoot();
            }
            if(InputUtil.LeftMouseButtonIsPressed) {
                gun.Hold();
            } else {
                gun.EndHold();
            }
        }
        if(CurrentDO != null) {
            Vector3 lookDir = CurrentDO.destination.transform.position - directionArrow.position;
            directionArrow.rotation = Quaternion.LookRotation(lookDir);

            if (InputUtil.LeftMouseButtonIsPressed) {
                ThrowStrength = Mathf.Min(1f, throwStrength + (Time.deltaTime * 0.8f));
            }
            if (InputUtil.LeftMouseButtonUp) {
                ThrowDeliveryObject();
                ThrowStrength = 0;
            }
        }
    }

    private void ThrowDeliveryObject() {
        lastThrowTime = Time.time;
        CurrentDO.mainT.SetParent(null, true);
        CurrentDO.mainRigidbody.isKinematic = false;
        CurrentDO.mainRigidbody.AddForce(mainCameraTransform.forward * 1500 * EasedThrowStrength);
        CurrentDO = null;
        AudioManager.Instance.PlaySFX(throwSound, 1f);
        directionArrow.gameObject.SetActive(false);
    }

    private bool gunEquipped = false;
    public bool GunEquipped {
        get { return gunEquipped; }
        set {
            if(gunEquipped != value) {
                gunEquipped = value;
                gun.SetEquipped(gunEquipped);
            }
        }
    }

    public void SetFPSControllerActive(bool isActive) {
        enabled = isActive;
        characterController.enabled = isActive;
        firstPersonController.enabled = isActive;
        Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isActive;

        if (isActive) {
            PauseManager.ReleasePause(this);
            if(CurrentDO == null && DayNightManager.instance.IsNight) {
                GunEquipped = true;
            }
        } else {
            PauseManager.RequestPause(this);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("DeliveryObject")) {
            if(CurrentDO == null &&  Time.time - lastThrowTime > 0.5f) {
                PickUpDeliveryObject(other.gameObject.GetComponent<DeliveryObject>());
            }
        }
    }

    public void DOHitDestination(DeliveryObject deliveryObject) {
        AudioManager.Instance.PlaySuccess();
        MoneyManager.instance.CurrentMoney += 10;
        if(deliveryObject == CurrentDO) {
            ThrowStrength = 0;
            CurrentDO = null;
            directionArrow.gameObject.SetActive(false);
        }
    }

    private void PickUpDeliveryObject(DeliveryObject deliveryObject) {
        GunEquipped = false;
        CurrentDO = deliveryObject;
        CurrentDO.mainRigidbody.isKinematic = true;
        CurrentDO.mainT.SetParent(mainCameraTransform, false);
        CurrentDO.mainT.localPosition = new Vector3(1.26f, -0.8f, 2.18f);
        CurrentDO.mainT.localEulerAngles = new Vector3(20, 200, -3);
        CurrentDO.RemoveFromSpawnLocation();
        AudioManager.Instance.PlaySFX(pickUpSound, 1f);
        directionArrow.gameObject.SetActive(true);
    }
}
