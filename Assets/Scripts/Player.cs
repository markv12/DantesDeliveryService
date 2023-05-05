using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public HeavyGun heavyGun;
    public int maxHealth;
    private int health;
    public int CurrentHealth => health; 

    public AudioClip pickUpSound;
    public AudioClip throwSound;
    public GameOverUI gameOverUI;

    private DeliveryObject currentDO;
    private DeliveryObject CurrentDO {
        get {
            return currentDO;
        }
        set {
            if (currentDO != value) {
                currentDO = value;
                DeliveryManager.instance.SetActiveDO(currentDO);
                if (currentDO == null && DayNightManager.instance.IsNight) {
                    TakeGunsOut();
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
        FullHeal();
    }

    private float lastHitTime;
    public void Hurt(int damage) {
        if(Time.time - lastHitTime > 1.2f) {
            lastHitTime = Time.time;
            health = Mathf.Max(0, health - damage);
            playerUI.ShowHealth(health, maxHealth);
            if (health > 0) {
                AudioManager.Instance.PlayPlayerHurt();
            } else {
                if (!died) {
                    Die();
                }
            }
        }
    }

    public void FullHeal() {
        health = maxHealth;
        playerUI.ShowHealth(health, maxHealth);
    }

    private bool died = false;
    private void Die() {
        died = true;
        AudioManager.Instance.PlayPlayerDie();
        AudioManager.Instance.FadeOutBGM();
        SetFPSControllerActive(false);
        gameOverUI.Show();
    }

    private float lastThrowTime;
    private void Update() {
        if (GunEquipped) {
            if (InputUtil.LeftMouseButtonDown) {
                gun.Shoot();
            }
            if (InputUtil.LeftMouseButtonIsPressed) {
                gun.Hold();
            } else {
                gun.EndHold();
            }
        } else if (HeavyGunEquipped) {
            if (InputUtil.LeftMouseButtonDown && heavyGun.CanShoot) {
                if (heavyGun.CanShoot) {
                    heavyGun.Shoot();
                } else {
                    AudioManager.Instance.PlayOutOfAmmoSound();
                }
            }
        }

        if (CurrentDO != null) {
            Vector3 lookDir = CurrentDO.destination.transform.position - directionArrow.position;
            directionArrow.rotation = Quaternion.LookRotation(lookDir);
            if (InputUtil.LeftMouseButtonIsPressed) {
                ThrowDeliveryObject();
            }
        }

        if (DayNightManager.instance.IsNight && CurrentDO == null && StatsManager.instance.shotgunUnlocked && InputUtil.GetKeyDown(Key.Q)) {
            gun.SetSwitchWeaponTextActive(false);
            SwitchGuns();
        }
    }

    private void ThrowDeliveryObject() {
        lastThrowTime = Time.time;
        CurrentDO.mainT.SetParent(null, true);
        CurrentDO.mainRigidbody.isKinematic = false;
        CurrentDO.mainRigidbody.AddForce(mainCameraTransform.forward * 1150);
        CurrentDO = null;
        AudioManager.Instance.PlaySFX(throwSound, 1f);
        directionArrow.gameObject.SetActive(false);
    }

    private bool gunEquipped = false;
    private bool GunEquipped {
        get { return gunEquipped; }
        set {
            if (gunEquipped != value) {
                gunEquipped = value;
                gun.SetEquipped(gunEquipped);
            }
        }
    }

    private bool heavyGunEquipped = false;
    private bool HeavyGunEquipped {
        get { return heavyGunEquipped; }
        set {
            if (heavyGunEquipped != value) {
                heavyGunEquipped = value;
                heavyGun.SetEquipped(heavyGunEquipped);
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
            if (CurrentDO == null && DayNightManager.instance.IsNight) {
                TakeGunsOut();
            }
        } else {
            PauseManager.RequestPause(this);
        }
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("DeliveryObject")) {
            if (CurrentDO == null && Time.time - lastThrowTime > 0.5f) {
                PickUpDeliveryObject(other.gameObject.GetComponent<DeliveryObject>());
            }
        }
    }

    public void DOHitDestination(DeliveryObject deliveryObject) {
        AudioManager.Instance.PlaySuccess();
        StatsManager.instance.AddMoney(StatsManager.instance.DelveryMoney);
        if (deliveryObject == CurrentDO) {
            CurrentDO = null;
            directionArrow.gameObject.SetActive(false);
        }
    }

    private void PickUpDeliveryObject(DeliveryObject deliveryObject) {
        PutGunsAway();
        CurrentDO = deliveryObject;
        CurrentDO.mainRigidbody.isKinematic = true;
        CurrentDO.mainT.SetParent(mainCameraTransform, false);
        CurrentDO.mainT.localPosition = new Vector3(1f, -0.7f, 2.18f);
        CurrentDO.mainT.localEulerAngles = new Vector3(20, 200, -3);
        CurrentDO.RemoveFromSpawnLocation();
        AudioManager.Instance.PlaySFX(pickUpSound, 1f);
        directionArrow.gameObject.SetActive(true);
    }

    private bool heavyGunSelected = false;
    private Coroutine switchRoutine;
    private void SwitchGuns() {
        this.EnsureCoroutineStopped(ref switchRoutine);
        switchRoutine = StartCoroutine(SwitchRoutine());

        IEnumerator SwitchRoutine() {
            heavyGunSelected = !heavyGunSelected;
            if (heavyGunSelected && GunEquipped) {
                GunEquipped = false;
            }
            if (!heavyGunSelected && HeavyGunEquipped) {
                HeavyGunEquipped = false;
            }
            yield return WaitUtil.GetWait(0.4f);
            if(CurrentDO == null) {
                if (heavyGunSelected && !HeavyGunEquipped) {
                    HeavyGunEquipped = true;
                }
                if (!heavyGunSelected && !GunEquipped) {
                    GunEquipped = true;
                }
            }
        }
    }

    public void PutGunsAway() {
        GunEquipped = false;
        HeavyGunEquipped = false;
    }

    private void TakeGunsOut() {
        if (heavyGunSelected) {
            HeavyGunEquipped = true;
        } else {
            GunEquipped = true;
        }
    }

    public void SetMoveSpeed(float moveSpeed) {
        firstPersonController.SetMoveSpeed(moveSpeed);
    }

    public void SetSwitchWeaponTextActive(bool active) {
        gun.SetSwitchWeaponTextActive(active);
    }
}
