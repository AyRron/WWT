using UnityEngine;

namespace Script.Script_Camera
{
    public class CameraController : MonoBehaviour
    {
        
        public Transform player; // Le tank
        public float smoothSpeed = 0.125f; // Vitesse de suivi de la caméra
        public Vector3 offset; // Décalage de la caméra par rapport au tank
        public float edgeSize = 10.0f; // Taille de la zone de déclenchement du déplacement de la caméra
        public float moveSpeed = 10.0f; // Vitesse de déplacement de la caméra
        public bool isFollowingPlayer = true;

        void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isFollowingPlayer = true;
            }
            else if (Input.mousePosition.x >= Screen.width - edgeSize ||
                     Input.mousePosition.x <= edgeSize ||
                     Input.mousePosition.y >= Screen.height - edgeSize ||
                     Input.mousePosition.y <= edgeSize)
            {
                isFollowingPlayer = false;
            }

            if (isFollowingPlayer)
            {
                FollowPlayer();
            }
            else
            {
                FreeMove();
            }
        }

        void FollowPlayer()
        {
            if (player != null)
            {
                Vector3 desiredPosition = player.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
                
                transform.LookAt(player);
            }
        }

        void FreeMove()
        {
            Vector3 moveDirection = Vector3.zero;

            if (Input.mousePosition.x >= Screen.width - edgeSize)
            {
                moveDirection += Vector3.right;
            }
            if (Input.mousePosition.x <= edgeSize)
            {
                moveDirection += Vector3.left;
            }
            if (Input.mousePosition.y >= Screen.height - edgeSize)
            {
                moveDirection += Vector3.forward;
            }
            if (Input.mousePosition.y <= edgeSize)
            {
                moveDirection += Vector3.back;
            }
            

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            
        }

    }
}