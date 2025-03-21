using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class TankMovement : MonoBehaviour
{
    // access to nav mesh Agent (floor)
    [SerializeField] private NavMeshAgent _agent = null;

    public LayerMask ground;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            float maxDistance = 100f;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance, ground))
            {
                _agent.SetDestination(hit.point);
            }
        }

    }
}
