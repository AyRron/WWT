using UnityEngine;
using UnityEngine.AI;

public class tankFollowState : StateMachineBehaviour
{
    AttackController attackController;

    NavMeshAgent agent;

    public float attackingDistance = 10f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Should Unit Transition to Idle state
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        } else
        {
            // If there is no other direct command to move
            if(animator.transform.GetComponent<TankMovement>().isCommandeToMove == false)
            {
                // Moving Unit towards 
                agent.SetDestination(attackController.targetToAttack.position);
                animator.transform.LookAt(attackController.targetToAttack);

                //float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);
                //if (distanceFromTarget < attackingDistance)
                //{
                //         agent.SetDestination(animator.transform.position);

                //    animator.SetBool("isAttacking", true);
                //}
            }

        }
     }

  

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
