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
    public PaintGun paintGun;

    public Vector3 FaceDirection => mainCameraTransform.forward;

    private Vector3 playerStartPos;
    private Quaternion playerStartRotation;

    private void Awake() {
        instance = this;
        mainCameraTransform = mainCamera.transform;
        playerStartPos = t.position;
        playerStartRotation = t.rotation;
        directionArrow.gameObject.SetActive(false);
    }

    private float throwStrength = 0;
    private float ThrowStrength {
        get {
            return throwStrength;
        }
        set {
            throwStrength = value;
            playerUI.ShowThrowStrength(throwStrength);
        }
    }

    private float lastThrowTime;
    private void Update() {
        if(InputUtil.GetKeyDown(Key.G)) {
            PaintGunEquipped = !PaintGunEquipped;
        }
        if (PaintGunEquipped) {
            if (InputUtil.LeftMouseButtonDown) {
                paintGun.Shoot();
            }
            if(InputUtil.LeftMouseButtonIsPressed) {
                paintGun.Hold();
            } else {
                paintGun.EndHold();
            }
        }
        if(currentDO != null) {
            Vector3 lookDir = currentDO.destination.transform.position - directionArrow.position;
            directionArrow.rotation = Quaternion.LookRotation(lookDir);

            if (InputUtil.LeftMouseButtonIsPressed) {
                ThrowStrength = Mathf.Min(1f, throwStrength + (Time.deltaTime * 0.5f));
            }
            if (InputUtil.LeftMouseButtonUp) {
                if(throwStrength > 0.05f) {
                    ThrowDeliveryObject();
                }
                ThrowStrength = 0;
            }
        }
    }

    private void ThrowDeliveryObject() {
        lastThrowTime = Time.time;
        currentDO.mainT.SetParent(null, true);
        currentDO.mainRigidbody.isKinematic = false;
        currentDO.mainRigidbody.AddForce(mainCameraTransform.forward * 1200 * throwStrength);
        currentDO = null;
        directionArrow.gameObject.SetActive(false);
    }

    private bool paintGunEquipped = false;
    private bool PaintGunEquipped {
        get { return paintGunEquipped; }
        set {
            if(paintGunEquipped != value) {
                paintGunEquipped = value;
                paintGun.SetEquipped(paintGunEquipped);
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
        } else {
            PauseManager.RequestPause(this);
        }
    }

    private DeliveryObject currentDO;
    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("DeliveryObject")) {
            if(Time.time - lastThrowTime > 0.5f) {
                PickUpDeliveryObject(other.gameObject.GetComponent<DeliveryObject>());
            }
        }
    }

    public void DOHitDestination(DeliveryObject deliveryObject) {
        if(deliveryObject == currentDO) {
            currentDO = null;
            directionArrow.gameObject.SetActive(false);
        }
    }

    private void PickUpDeliveryObject(DeliveryObject deliveryObject) {
        currentDO = deliveryObject;
        currentDO.mainRigidbody.isKinematic = true;
        currentDO.mainT.SetParent(mainCameraTransform, false);
        currentDO.mainT.localPosition = new Vector3(0, -0.55f, 2f);
        directionArrow.gameObject.SetActive(true);
    }
}
