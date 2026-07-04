using UnityEngine;
using System.Collections;

namespace Invector.vCharacterController
{
    public class vThirdPersonInputCustom : vThirdPersonInput
    {
        [Header("Combat Input")]
        public KeyCode headbuttInput = KeyCode.R;
        public KeyCode uppercutInput = KeyCode.E;
        public KeyCode leftHookInput = KeyCode.Q;
        public KeyCode crouchInput = KeyCode.LeftControl;

        private CombatManager combat;
        private bool isExitingCrouch = false;

        protected override void Start()
        {
            base.Start();
            combat = GetComponent<CombatManager>();
        }

        protected override void InputHandle()
        {
            MovementInput();
            CombatInput();
        }

        private void MovementInput()
        {
            MoveInput();
            CameraInput();

            CrouchInput();

            if (combat.IsCrouching || isExitingCrouch)
            {
                cc.input = Vector3.zero;
                cc.inputSmooth = Vector3.zero;
                cc.moveDirection = Vector3.zero;
                cc._rigidbody.velocity = new Vector3(0f, cc._rigidbody.velocity.y, 0f);
                return;
            }

            if (combat.CanSprint) SprintInput();
            if (combat.CanJump) JumpInput();

            StrafeInput();
        }

        private void CombatInput()
        {
            // Blokir semua combat input (termasuk dodge) saat airborne
            if (!cc.isGrounded || cc.isJumping)
                return;

            // Flying Kick: Shift + W + Left Click
            if (Input.GetMouseButtonDown(0) && combat.CanFlyingKick && IsFlyingKickInput())
            {
                combat.PerformAction(CombatAction.FlyingKick);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (combat.IsCrouching)
                    combat.ExitCrouch();

                // Nonaktifkan strafe hanya jika dodge bukan ke depan
                if (cc.isStrafing && !IsDodgingForward())
                    cc.Strafe();

                //dodge selesai = strafe mode aktif kembali
                //if (cc.isStrafing)
                //{
                //    combat.SetWasStrafing(true); // simpan bahwa dodge dari strafe
                //    cc.Strafe(); // nonaktifkan strafe
                //}
                //else
                //{
                //    combat.SetWasStrafing(false);
                //}

                combat.PerformAction(CombatAction.Dodge);
                return;
            }

            if (!combat.CanAttack) return;

            if (Input.GetMouseButtonDown(0))
            {
                combat.PerformAction(CombatAction.Punch);
                return;
            }
            if (Input.GetKeyDown(headbuttInput))
            {
                combat.PerformAction(CombatAction.Headbutt);
                return;
            }
            if (Input.GetKeyDown(uppercutInput))
            {
                combat.PerformAction(CombatAction.Uppercut);
                return;
            }
            if (Input.GetKeyDown(leftHookInput))
            {
                combat.PerformAction(CombatAction.LeftHook);
                return;
            }
        }

        private bool IsFlyingKickInput()
        {
            // Harus sprint + input ke depan (W)
            return cc.isSprinting && cc.input.z > 0.5f && Mathf.Abs(cc.input.x) < 0.5f;
        }

        private bool IsDodgingForward()
        {
            // Cek input vertical positif (ke depan) dan horizontal mendekati nol
            return cc.input.z > 0.1f && Mathf.Abs(cc.input.x) < 0.5f;
        }

        private void CrouchInput()
        {
            if (!Input.GetKeyDown(crouchInput)) return;

            if (combat.IsCrouching)
                StartCoroutine(ExitCrouchRoutine());
            else
                combat.EnterCrouch();
        }

        private IEnumerator ExitCrouchRoutine()
        {
            combat.ExitCrouch();
            isExitingCrouch = true;

            // Tunggu 1 frame agar animator update
            yield return null;

            yield return new WaitUntil(() =>
            {
                var stateInfo = cc.animator.GetCurrentAnimatorStateInfo(0);
                return !stateInfo.IsName("CrouchExit") && !cc.animator.IsInTransition(0);
            });

            isExitingCrouch = false;
        }
    }
}