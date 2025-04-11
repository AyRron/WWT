using UnityEngine;
using UnityEngine.AI;

public class tankFollowState : StateMachineBehaviour
{
    AttackController attackController;

    NavMeshAgent agent;

    public float attackingDistance = 10f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        attackController = animator.GetComponent<AttackController>();
        agent = animator.transform.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

        // Should Unit Transition to Idle state
        if (attackController.targetToAttack == null)
        {
            animator.SetBool("isFollowing", false);
        }
        else
        {
            float distanceFromTarget = Vector3.Distance(attackController.targetToAttack.position, animator.transform.position);

            if (distanceFromTarget < attackingDistance)
            {

                agent.SetDestination(animator.transform.position);

                animator.SetBool("isAttacking", true);
            }

            // If there is no other direct command to move
            if (animator.transform.GetComponent<TankMovement>().isCommandToMove == false)
            {
                agent.SetDestination(attackController.targetToAttack.position);

                Vector3 direction = attackController.targetToAttack.position - animator.transform.position;

                direction.y = 0;

                if (direction.sqrMagnitude > 0.01f)  
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    animator.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                }
            }
        }
    }



    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
