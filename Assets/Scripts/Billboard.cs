using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCamera)
        {
            var direction = _mainCamera.transform.position - transform.position;
            direction.y = 0;
            direction.x = 0;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-direction);
            }
        }
    }
}