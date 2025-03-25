using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class move : MonoBehaviour
{
    // access to nav mesh Agent (floor)
    [SerializeField] private NavMeshAgent _agent = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
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
}
