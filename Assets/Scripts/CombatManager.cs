using UnityEngine;

namespace Invector.vCharacterController
{
    public class CombatManager : MonoBehaviour
    {
        #region Variables

        private vThirdPersonController controller;
        private Animator animator;
        //dodge selesai = strafe mode aktif kembali
        //private bool _wasStrafingBeforeDodge = false;

        public PlayerState CurrentState { get; private set; }

        public Animator Animator => animator;
        public vThirdPersonController Controller => controller;

        #endregion

        #region State Properties

        public bool CanMove =>
            CurrentState == PlayerState.Normal ||
            CurrentState == PlayerState.Dodge ||
            CurrentState == PlayerState.Crouch;

        public bool CanAttack =>
            CurrentState == PlayerState.Normal &&
            controller.isGrounded &&
            !controller.isJumping;

        public bool CanJump =>
            CurrentState == PlayerState.Normal;

        public bool CanSprint =>
            CurrentState == PlayerState.Normal;

        public bool CanRotate =>
            CurrentState == PlayerState.Normal ||
            CurrentState == PlayerState.Dodge;

        public bool IsCrouching =>
            CurrentState == PlayerState.Crouch;

        public bool CanFlyingKick =>
            CurrentState == PlayerState.Normal &&
            controller.isGrounded &&
            !controller.isJumping &&
            controller.isSprinting;

        #endregion

        #region Unity

        private void Awake()
        {
            controller = GetComponent<vThirdPersonController>();
            animator = GetComponent<Animator>();
            SetState(PlayerState.Normal);
        }

        #endregion

        #region Public API

        public bool PerformAction(CombatAction action)
        {
            if (!CanPerformAction(action))
                return false;

            PrepareAction(action);
            TriggerAction(action);
            return true;
        }

        public void BeginAction() { }

        public void EndAction()
        {
            SetState(PlayerState.Normal);
        }

        public void BeginDodge()
        {
            controller.StopMovementImmediately();
            SetState(PlayerState.Dodge);
        }

        //dodge selesai = strafe mode aktif kembali
        //public void SetWasStrafing(bool value)
        //{
        //    _wasStrafingBeforeDodge = value;
        //}

        public void EndDodge()
        {
            // Selalu kembali ke Normal setelah dodge, tidak peduli dari state apa
            SetState(PlayerState.Normal);

            // Aktifkan kembali strafe jika dodge berasal dari strafe mode
            //if (_wasStrafingBeforeDodge)
            //{
            //    _wasStrafingBeforeDodge = false;
            //    controller.Strafe();
            //}
        }

        public void EnterCrouch()
        {
            if (CurrentState != PlayerState.Normal) return;
            SetState(PlayerState.Crouch);
            animator.SetBool(AnimatorParameters.Crouch, true);
            // Tambah sementara:
            Debug.Log($"lockMovement = {controller.lockMovement}");
        }

        public void ExitCrouch()
        {
            if (CurrentState != PlayerState.Crouch) return;
            SetState(PlayerState.Normal);
            animator.SetBool(AnimatorParameters.Crouch, false);
        }

        #endregion

        #region Internal

        private bool CanPerformAction(CombatAction action)
        {
            return CurrentState == PlayerState.Normal;
        }

        private void PrepareAction(CombatAction action)
        {
            switch (action)
            {
                case CombatAction.Dodge:
                    controller.ClearMovementInput();
                    break;
                default:
                    controller.StopMovementImmediately();
                    break;
            }
        }

        private void TriggerAction(CombatAction action)
        {
            animator.SetTrigger(GetAnimatorTrigger(action));
        }

        private int GetAnimatorTrigger(CombatAction action)
        {
            switch (action)
            {
                case CombatAction.Punch: return AnimatorParameters.Punch;
                case CombatAction.Headbutt: return AnimatorParameters.Headbutt;
                case CombatAction.Uppercut: return AnimatorParameters.Uppercut;
                case CombatAction.LeftHook: return AnimatorParameters.LeftHook;
                case CombatAction.Dodge: return AnimatorParameters.Dodge;
                case CombatAction.FlyingKick: return AnimatorParameters.FlyingKick;
                default: return 0;
            }
        }

        #endregion

        #region State

        public void SetState(PlayerState state)
        {
            if (CurrentState == state) return;

            CurrentState = state;

            switch (state)
            {
                case PlayerState.Normal:
                    controller.lockMovement = false;
                    controller.lockRotation = false;
                    break;

                case PlayerState.Crouch:
                    controller.lockMovement = true;  // tidak bisa gerak saat crouch idle
                    controller.lockRotation = true; // kamera tetap bisa rotate
                    break;

                case PlayerState.Attack:
                    controller.lockMovement = true;
                    controller.lockRotation = true;
                    break;

                case PlayerState.Dodge:
                    controller.lockMovement = false;
                    controller.lockRotation = false;
                    break;

                case PlayerState.Hit:
                    controller.lockMovement = true;
                    controller.lockRotation = true;
                    break;

                case PlayerState.Dead:
                    controller.lockMovement = true;
                    controller.lockRotation = true;
                    break;
                case PlayerState.FlyingKick:
                    controller.lockMovement = true;  // root motion yang handle movement
                    controller.lockRotation = true;
                    break;
            }

#if UNITY_EDITOR
            Debug.Log($"Player State -> {CurrentState}");
#endif
        }

        #endregion
    }
}