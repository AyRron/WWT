using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class move : MonoBehaviour
{

    public float speed = 4f;
    public Vector3 target;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.target = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
    }
}
