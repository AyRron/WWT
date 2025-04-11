using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;
    public Material idleStateMaterial;
    public Material followStateMaterial;
    public Material attackStateMaterial;
    public TankHealth tankHealftTarget;
    public List<GameObject> enemyList = new List<GameObject>();
    public string enemyTag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            GameObject newEnemy = other.gameObject;

            // On ajoute l'ennemi � la liste s'il n'est pas d�j� dedans
            if (!enemyList.Contains(newEnemy))
            {
                enemyList.Add(newEnemy);
            }

            // Tente d'assigner une cible valide si aucune n'est s�lectionn�e
            if (targetToAttack == null)
            {
                AssignNextValidTarget();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            GameObject exitingEnemy = other.gameObject;

            // Retirer l'ennemi de la liste
            enemyList.Remove(exitingEnemy);

            // Si l'ennemi qui sortait etait la cible actuelle, on cherche un autre
            if (targetToAttack != null && targetToAttack.gameObject == exitingEnemy)
            {
                targetToAttack = null;
                tankHealftTarget = null;
                AssignNextValidTarget();
            }
        }
    }

    private void Update()
    {
        // Verifie si la cible actuelle est morte, et passe a la suivante si oui
        if (tankHealftTarget != null && tankHealftTarget._currentHealth <= 0)
        {
            enemyList.Remove(targetToAttack.gameObject);
            targetToAttack = null;
            tankHealftTarget = null;
            AssignNextValidTarget();
        }
    }

    private void AssignNextValidTarget()
    {
        foreach (GameObject enemy in enemyList)
        {
            TankHealth health = enemy.GetComponent<TankHealth>();
            if (health != null && health._currentHealth > 0)
            {
                targetToAttack = enemy.transform;
                tankHealftTarget = health;
                break;
            }
        }
    }
}
