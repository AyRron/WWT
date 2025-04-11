using UnityEngine;

public class tankIdleState : StateMachineBehaviour
{
    AttackController attackController;

    public float followingDistance = 10f;

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

            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);


            if (followingDistance < distanceFromTarget)
            {
                animator.SetBool("isFollowing", true);
            }

        }

    }

}
