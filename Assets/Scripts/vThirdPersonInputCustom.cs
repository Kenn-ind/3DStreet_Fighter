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
            if (Input.GetMouseButtonDown(1))
            {
                if (combat.IsCrouching)
                    combat.ExitCrouch();

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