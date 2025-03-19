using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class move : MonoBehaviour
{
    // access to nav mesh Agent (floor)
    [SerializeField] private NavMeshAgent _agent = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _agent.SetDestination(hit.point);
            }
        }

    }
}
