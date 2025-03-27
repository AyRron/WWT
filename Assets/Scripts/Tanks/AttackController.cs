using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && targetToAttack == null)
        {
            Debug.Log("rentre ");
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("sort ");
        if (other.CompareTag("Enemy") && targetToAttack != null) 
        {
            targetToAttack = null;
        }
    }
}
