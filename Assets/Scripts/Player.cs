using System;
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

    public PaintGun paintGun;

    public Vector3 FaceDirection => mainCameraTransform.forward;

    private Vector3 playerStartPos;
    private Quaternion playerStartRotation;

    private void Awake() {
        instance = this;
        mainCameraTransform = mainCamera.transform;
        playerStartPos = t.position;
        playerStartRotation = t.rotation;
    }

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

    private void SetFPSControllerActive(bool isActive) {
        enabled = isActive;
        characterController.enabled = isActive;
        firstPersonController.enabled = isActive;
        Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isActive;
    }

    private DeliveryObject currentDeliveryObject;
    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("DeliveryObject")) {
            currentDeliveryObject = other.gameObject.GetComponent<DeliveryObject>();
            currentDeliveryObject.mainT.SetParent(mainCameraTransform, false);
            currentDeliveryObject.mainT.localPosition = new Vector3(0, -0.55f, 2f);
        } else if (other.gameObject.CompareTag("Destination")) {
            Destination destination = other.gameObject.GetComponent<Destination>();
            if(destination != null && currentDeliveryObject != null && currentDeliveryObject.destination == destination) {
                Destroy(currentDeliveryObject.gameObject);
                currentDeliveryObject = null;
            }
        }
    }
}
