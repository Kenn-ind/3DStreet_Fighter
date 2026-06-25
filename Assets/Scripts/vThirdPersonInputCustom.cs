using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Rendering.Universal;

namespace Invector.vCharacterController
{
    public class vThirdPersonInputCustom : vThirdPersonInput
    {
        [Header("Combat Input")]
        public KeyCode headbuttInput = KeyCode.R;
        public KeyCode uppercutInput = KeyCode.E;
        public KeyCode leftHookInput = KeyCode.Q;
        public KeyCode crouchInput = KeyCode.LeftAlt;

        private CombatManager combat;

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

            if (combat.CanSprint)
                SprintInput();

            if (combat.CanJump)
                JumpInput();

            StrafeInput();

            CrouchInput();
        }

        private void CombatInput()
        {
            if (!combat.CanAttack)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                combat.PerformAction(CombatAction.Punch);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                combat.PerformAction(CombatAction.Dodge);
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
            if (Input.GetKeyDown(crouchInput) ||
                Input.GetKeyDown(KeyCode.RightAlt))
            {
                Debug.Log("lagi berak");
                cc.animator.SetBool("isCrouching", true);
            }

            if (Input.GetKeyUp(crouchInput) ||
                Input.GetKeyUp(KeyCode.RightAlt))
            {
                cc.animator.SetBool("isCrouching", false);
            }
        }
    }
}