using UnityEngine;

namespace Invector.vCharacterController
{
    public class FlyingKickStateBehaviour : StateMachineBehaviour
    {
        private CombatManager combat;

        public override void OnStateEnter(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (combat == null)
                combat = animator.GetComponent<CombatManager>();

            combat.SetState(PlayerState.FlyingKick);
        }

        public override void OnStateExit(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            combat.SetState(PlayerState.Normal);
        }
    }
}
