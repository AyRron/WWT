using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using TMPro;
using UnityEngine.EventSystems;

public class TankMovement : MonoBehaviour
{
    // access to nav mesh Agent (floor)
    [SerializeField] private NavMeshAgent _agent = null;

    public LayerMask ground;
    public bool isCommandeToMove;


    private void Start()
    {
        _agent.updateRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
                float maxDistance = Mathf.Infinity;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxDistance, ground)){
                    isCommandeToMove = true;
                    _agent.SetDestination(hit.point);
                }  
        }

        // Agent reached destination
        if(_agent.hasPath == false || _agent.remainingDistance == _agent.stoppingDistance)
        {
            isCommandeToMove=false;
        }



    }

    
}
