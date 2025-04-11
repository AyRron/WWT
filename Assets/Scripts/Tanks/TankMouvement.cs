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
    private bool _hasSetDestination = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                isCommandeToMove = true;
                _agent.SetDestination(hit.point);
                _hasSetDestination = true;
            }
        }

        //_hasSetDestination == true → c’est un mouvement qu’on a déclenché, pas un hasard
        //_agent.pathPending == false → Unity a fini de calculer le chemin
        //_agent.remainingDistance <= _agent.stoppingDistance → Le tank est réellement arrivé

        if (_hasSetDestination && !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            isCommandeToMove = false;
            _hasSetDestination = false;
        }
    }

}
