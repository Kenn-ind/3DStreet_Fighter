using UnityEngine;

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

            // CrouchInput harus selalu dipanggil, sebelum return
            CrouchInput();

            // Blok input movement saat crouch
            if (combat.IsCrouching)
            {
                cc.input = Vector3.zero;
                cc.inputSmooth = Vector3.zero;
                cc.moveDirection = Vector3.zero;
                return; // skip sprint, jump, strafe
            }

            if (combat.CanSprint) SprintInput();
            if (combat.CanJump) JumpInput();

            StrafeInput();
        }

        private void CombatInput()
        {
            // Dodge: bisa dari Normal atau Crouch
            if (Input.GetMouseButtonDown(1))
            {
                if (combat.IsCrouching)
                    combat.ExitCrouch(); // → state jadi Normal dulu

                combat.PerformAction(CombatAction.Dodge); // CanPerformAction cek Normal ✓
                return;
            }

            // Serangan lain: hanya dari Normal
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
                combat.ExitCrouch();
            else
                combat.EnterCrouch();
        }
    }
}