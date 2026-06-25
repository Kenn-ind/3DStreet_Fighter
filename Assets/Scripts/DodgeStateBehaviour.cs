using UnityEngine;

namespace Invector.vCharacterController
{
    public class DodgeStateBehaviour : StateMachineBehaviour
    {
        private CombatManager combat;

        public override void OnStateEnter(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (combat == null)
                combat = animator.GetComponent<CombatManager>();

            combat.BeginDodge();
        }

        public override void OnStateExit(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            combat.EndDodge();
        }
    }
}