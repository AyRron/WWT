using UnityEngine;

public class tankIdleState : StateMachineBehaviour
{
    AttackController attackController;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if there is an available target
        if (attackController.targetToAttack != null) { 

            // transition to Follow State
            animator.SetBool("isFollowing", true);
        }

    }

}
