using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class tankAttackState : StateMachineBehaviour
{
    private NavMeshAgent _agent;
    private AttackController _attackController;
    private Transform _tankTurret;
    private TankShooting _tankShooting;

    public float stopAttackingDistance = 1.2f;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _agent = animator.GetComponent<NavMeshAgent>();
        _attackController = animator.GetComponent<AttackController>();

        Transform tankVisual = animator.transform.Find("TankVisual");
        Transform tankBody = tankVisual.Find("Body");
        _tankTurret = tankBody.Find("Turret");

        _tankShooting = animator.GetComponent<TankShooting>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

        if (_attackController.targetToAttack != null)
        {
            bool hasCommandeToMove = false;

            // Vérifie s’il y a un TankMovement (joueur)
            TankMovement playerMovement = animator.transform.GetComponent<TankMovement>();
            if (playerMovement != null)
            {
                hasCommandeToMove = playerMovement.isCommandeToMove;
            }
            else
            {
                // Sinon, vérifie s’il y a un TankEnemyMovement (IA)
                TankEnemyMovement enemyMovement = animator.transform.GetComponent<TankEnemyMovement>();
                if (enemyMovement != null)
                {
                    hasCommandeToMove = enemyMovement.isCommandeToMove;
                }
            }

            if (!hasCommandeToMove)
            {
                LookAtTarget();
                _tankShooting.Shoot();

                float distanceFromTarget = Vector3.Distance(_attackController.targetToAttack.position, animator.transform.position);

                if (distanceFromTarget > stopAttackingDistance || _attackController.targetToAttack == null)
                {
                    animator.SetBool("isAttacking", false);
                }
            }
        }
    }



    private void LookAtTarget()
    {
        if (_tankTurret == null || _attackController.targetToAttack == null)
        {
            Debug.LogWarning("Tourelle ou cible non trouvée !");
            return;
        }

        // Obtenir la direction vers la cible dans l'espace monde
        Vector3 directionToTarget = _attackController.targetToAttack.position - _tankTurret.position;

        // Si la direction est nulle, éviter une erreur
        if (directionToTarget.sqrMagnitude < 0.001f)
        {
            return;
        }

        // Convertir la direction en espace local (prendre en compte l'orientation du tank)
        Vector3 localDirection = _tankTurret.parent.InverseTransformDirection(directionToTarget);

        // Calculer l'angle dans le plan XY (ou XZ selon ton modèle)
        float angleZ = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

        float targetAngle = angleZ + 180f;
        float smoothAngle = Mathf.LerpAngle(_tankTurret.localRotation.eulerAngles.z, targetAngle, 0.3f);
        _tankTurret.localRotation = Quaternion.Euler(0, 0, smoothAngle);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _agent.isStopped = false;
    }

}
