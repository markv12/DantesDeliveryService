using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

#pragma warning disable 618, 649
namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField][Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private Camera m_Camera;
        private bool m_Jump;
        private float superJumpSpeed = -1;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded = true;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;

        // Use this for initialization
        private void Start() {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            ResetMouseLook();
        }

        public void ResetMouseLook() {
            m_MouseLook.Init(transform, m_Camera.transform);
        }


        // Update is called once per frame
        private void Update() {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump) {
                m_Jump = Keyboard.current.spaceKey.wasPressedThisFrame;
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded) {
                StartCoroutine(m_JumpBob.DoBobCycle());
                if (Time.timeSinceLevelLoad > 0.5f) {
                    PlayLandingSound();
                }
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded) {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound() {
            AudioManager.Instance.PlaySFX(m_LandSound, 1);
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate() {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;


            if (m_CharacterController.isGrounded) {
                m_MoveDir.y = -m_StickToGroundForce;

                if(superJumpSpeed > 0) {
                    Jump(superJumpSpeed);
                } else if (m_Jump) {
                    Jump(m_JumpSpeed);
                }
            } else {
                m_MoveDir += m_GravityMultiplier * Time.fixedDeltaTime * Physics.gravity;
            }
            if (m_CharacterController.enabled) {
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
            }

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }

        private void Jump(float speed) {
            m_MoveDir.y = speed;
            PlayJumpSound();
            m_Jump = false;
            superJumpSpeed = -1;
            m_Jumping = true;
        }

        public void SuperJump(float speed) {
            superJumpSpeed = speed;
        }

        private void PlayJumpSound() {
            AudioManager.Instance.PlaySFX(m_JumpSound, 1);
        }


        private void ProgressStepCycle(float speed) {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0)) {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep)) {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio() {
            if (!m_CharacterController.isGrounded) {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            AudioClip stepClip = m_FootstepSounds[n];
            AudioManager.Instance.PlaySFX(stepClip, 1);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = stepClip;
        }


        private void UpdateCameraPosition(float speed) {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob) {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded) {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            } else {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed) {
            // Read input
            float horizontal = InputUtil.GetHorizontal();
            float vertical = InputUtil.GetVertical();
            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Keyboard.current.leftShiftKey.isPressed;
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1) {
                m_Input.Normalize();
            }
        }


        private void RotateView() {
            if (Time.timeSinceLevelLoad > 0.5f) {
                m_MouseLook.LookRotation(transform, m_Camera.transform);
            }
        }


        private void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below) {
                return;
            }

            if (body == null || body.isKinematic) {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
