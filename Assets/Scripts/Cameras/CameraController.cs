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

        private void Start()
        {
            // Chercher automatiquement le tank "Player" au démarrage
            if (player) return;

            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("Aucun objet avec le tag 'Player' n'a été trouvé !");
            }
        }

        private void Update()
        {
            if (!GameManager.Instance || !GameManager.Instance.gameRunning) return;

            // Refocus sur le tank avec la touche Espace
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isFollowingPlayer = true;
                if (player != null)
                {
                    // Position immédiate de la caméra
                    transform.position = player.position + offset;
                    transform.LookAt(player);
                }
            }

            // Basculer vers le mode libre si la souris est près des bords
            if (Input.mousePosition.x >= Screen.width - edgeSize ||
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

        private void FollowPlayer()
        {
            if (player)
            {
                Vector3 desiredPosition = player.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;

                transform.LookAt(player);
            }
            else
            {
                Debug.LogWarning("Aucun tank assigné pour le suivi de la caméra !");
            }
        }

        private void FreeMove()
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