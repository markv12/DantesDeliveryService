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
}
