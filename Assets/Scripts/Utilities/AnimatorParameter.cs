using UnityEngine;

namespace Invector.vCharacterController
{
    public static class AnimatorParameters
    {
        public static readonly int Punch = Animator.StringToHash("isPunching");

        public static readonly int Headbutt = Animator.StringToHash("isHeadbutt");

        public static readonly int Uppercut = Animator.StringToHash("isUppercut");

        public static readonly int LeftHook = Animator.StringToHash("isLeftHook");

        public static readonly int Dodge = Animator.StringToHash("isDodging");

        public static readonly int Crouch = Animator.StringToHash("isCrouching");
    }
}