using UnityEngine;
using System.Collections.Generic;

public class EnemyTankController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 120f;
    public float stoppingDistance = 0.1f;
    public float targetReachedDistance = 1f; // Distance à laquelle on considère avoir atteint la cible
    public Transform target;
    public Pathfinder pathfinder;
    public float pathUpdateRate = 0.5f;
    public float minDistanceToUpdatePath = 2f; // Distance minimale pour mettre à jour le chemin
    public float rotationThreshold = 10f; // Angle maximum de rotation avant de commencer à avancer
    public float tankRadius = 1f; // Rayon du tank pour la vérification des collisions
    public Vector3 tankSize = new Vector3(2f, 1f, 3f); // Taille du tank (largeur, hauteur, longueur)
    public float safetyMargin = 0.5f; // Marge de sécurité pour éviter les collisions
    public float obstacleAvoidanceRadius = 2f; // Rayon pour détecter les obstacles à éviter
    public float groundOffset = 0.1f; // Offset pour maintenir le tank au-dessus du sol
    public float stuckThreshold = 0.1f; // Distance minimale pour considérer que le tank a bougé
    public float maxStuckTime = 3f; // Temps maximum avant de considérer le tank comme bloqué
    public float unstuckMoveDistance = 3f; // Distance de déplacement pour les tentatives de déblocage

    private List<Vector3> path;
    private int currentPathIndex;
    private float nextPathUpdate;
    private Vector3 currentTargetPosition;
    private bool isRotating = false;
    private bool isInitialized = false;
    private Vector3 lastValidPosition;
    private float initialY; // Position Y initiale du tank
    private Vector3 lastPosition; // Dernière position du tank
    private float stuckTime = 0f; // Temps pendant lequel le tank est bloqué
    private bool isStuck = false; // Indique si le tank est bloqué
    private bool isUnstucking = false; // Indique si le tank est en train de se débloquer
    private float unstuckTimer = 0f; // Timer pour les actions de déblocage
    private float unstuckActionDuration = 1f; // Durée d'une action de déblocage
    private Vector3 unstuckDirection; // Direction pour se débloquer

    void Start()
    {
        // Mesurer la taille du tank automatiquement
        MeasureTankSize();
        
        InitializePathfinder();
        lastValidPosition = transform.position;
        lastPosition = transform.position;
        initialY = transform.position.y; // Sauvegarder la position Y initiale
    }

    // Mesure la taille du tank en utilisant les composants de collision ou le Renderer
    void MeasureTankSize()
    {
        bool sizeMeasured = false;
        
        // Essayer d'abord d'utiliser un BoxCollider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            // Utiliser la taille du BoxCollider
            tankSize = Vector3.Scale(boxCollider.size, transform.localScale);
            tankRadius = Mathf.Max(tankSize.x, tankSize.z) / 2f;
            sizeMeasured = true;
        }

        // Essayer ensuite d'utiliser un CapsuleCollider
        if (!sizeMeasured)
        {
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            if (capsuleCollider != null)
            {
                // Utiliser la taille du CapsuleCollider
                float radius = capsuleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);
                float height = capsuleCollider.height * transform.localScale.y;
                tankSize = new Vector3(radius * 2f, height, radius * 2f);
                tankRadius = radius;
                sizeMeasured = true;
            }
        }

        // Essayer ensuite d'utiliser un SphereCollider
        if (!sizeMeasured)
        {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider != null)
            {
                // Utiliser la taille du SphereCollider
                float radius = sphereCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                tankSize = new Vector3(radius * 2f, radius * 2f, radius * 2f);
                tankRadius = radius;
                sizeMeasured = true;
            }
        }

        // Si aucun collider n'est trouvé, essayer d'utiliser le Renderer
        if (!sizeMeasured)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                // Utiliser les limites du Renderer
                Bounds bounds = renderer.bounds;
                tankSize = bounds.size;
                tankRadius = Mathf.Max(tankSize.x, tankSize.z) / 2f;
                sizeMeasured = true;
            }
        }

        // Si aucun composant n'est trouvé, utiliser les valeurs par défaut
        if (!sizeMeasured)
        {
            // Les valeurs par défaut sont déjà définies dans les variables publiques
        }
        
        // Ajouter une marge de sécurité à la taille mesurée
        tankSize += Vector3.one * safetyMargin;
    }

    void InitializePathfinder()
    {
        if (pathfinder == null)
            pathfinder = GetComponent<Pathfinder>();
        
        if (pathfinder != null)
        {
            pathfinder.seeker = transform;
            pathfinder.target = target;
            
            // Transmettre la taille du tank au pathfinder
            pathfinder.agentRadius = tankRadius;
            pathfinder.agentSize = tankSize;
            
            // Si le pathfinder a une référence à la grille, mettre à jour la taille de l'agent
            if (pathfinder.grid != null)
            {
                pathfinder.grid.agentRadius = tankRadius;
                pathfinder.grid.agentSize = tankSize;
                pathfinder.grid.safetyMargin = safetyMargin;
            }
        }
        
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized)
        {
            InitializePathfinder();
            return;
        }

        if (target == null || pathfinder == null)
        {
            return;
        }

        // Maintenir le tank au-dessus du sol
        MaintainGroundPosition();

        // Vérifier si on a atteint la cible finale
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= targetReachedDistance)
        {
            isRotating = false;
            isStuck = false;
            isUnstucking = false;
            return;
        }

        // Vérifier si le tank est bloqué
        CheckIfStuck();

        // Si le tank est en train de se débloquer, continuer l'action de déblocage
        if (isUnstucking)
        {
            ContinueUnstuckAction();
            return;
        }

        // Mise à jour du chemin si nécessaire
        if (Time.time >= nextPathUpdate || path == null || path.Count == 0 || isStuck)
        {
            UpdatePath();
        }

        // Vérifier si le pathfinder a trouvé un chemin valide
        if (pathfinder.pathFound && path != null && path.Count > 0)
        {
            MoveAlongPath();
        }
        else
        {
            // Si pas de chemin valide, essayer de se diriger directement vers la cible
            DirectMoveToTarget();
        }
    }

    // Maintenir le tank au-dessus du sol
    void MaintainGroundPosition()
    {
        // Vérifier si le tank s'est enfoncé dans le sol
        if (transform.position.y < initialY - groundOffset)
        {
            // Remonter le tank à sa position Y initiale
            Vector3 newPosition = transform.position;
            newPosition.y = initialY;
            transform.position = newPosition;
        }
    }

    void UpdatePath()
    {
        nextPathUpdate = Time.time + pathUpdateRate;
        if (pathfinder.grid != null)
        {
            path = pathfinder.grid.path;
            if (path != null && path.Count > 0)
            {
                currentPathIndex = 0;
                isRotating = true;
            }
        }
    }

    void MoveAlongPath()
    {
        if (currentPathIndex >= path.Count)
        {
            return;
        }

        // Obtenir le point cible actuel et maintenir sa hauteur Y
        currentTargetPosition = path[currentPathIndex];
        currentTargetPosition.y = initialY;
        
        // Calculer la direction vers la cible
        Vector3 directionToTarget = currentTargetPosition - transform.position;
        directionToTarget.y = 0;

        // Passer au point suivant si on est trop proche du point actuel
        if (directionToTarget.magnitude < 0.001f)
        {
            currentPathIndex++;
            return;
        }

        // Calculer l'angle entre la direction actuelle et la direction vers la cible
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        // Rotation vers la cible
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            isRotating = angleToTarget > rotationThreshold;
        }

        // Déplacement vers la cible si on est bien orienté
        if (!isRotating)
        {
            float distanceToCurrentTarget = Vector3.Distance(transform.position, currentTargetPosition);
            
            if (distanceToCurrentTarget > stoppingDistance)
            {
                // Calculer la prochaine position
                Vector3 nextPosition = Vector3.MoveTowards(
                    transform.position,
                    currentTargetPosition,
                    moveSpeed * Time.deltaTime
                );
                nextPosition.y = initialY;
                
                // Vérifier si la position est accessible
                if (pathfinder.grid.IsPositionWalkable(nextPosition, tankSize, transform.rotation))
                {
                    transform.position = nextPosition;
                }
                else
                {
                    // Essayer de contourner l'obstacle
                    TryAvoidObstacle();
                }
            }
            else
            {
                // Passer au point suivant
                currentPathIndex++;
                isRotating = true;
            }
        }
    }

    void DirectMoveToTarget()
    {
        if (target == null) return;
        
        // Calculer la direction vers la cible
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0;
        
        if (directionToTarget.magnitude < 0.001f) return;
        
        // Rotation vers la cible
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        
        // Déplacement vers la cible si on est bien orienté
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget < rotationThreshold)
        {
            // Calculer la prochaine position
            Vector3 nextPosition = transform.position + transform.forward * moveSpeed * Time.deltaTime;
            nextPosition.y = initialY;
            
            // Vérifier si la position est accessible
            if (pathfinder.grid.IsPositionWalkable(nextPosition, tankSize, transform.rotation))
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(target.position.x, initialY, target.position.z),
                    moveSpeed * Time.deltaTime
                );
            }
            else
            {
                // Essayer de contourner l'obstacle
                TryAvoidObstacle();
            }
        }
    }

    void TryAvoidObstacle()
    {
        // Chercher une direction alternative pour contourner l'obstacle
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;
            nextPosition.y = initialY;
            
            if (pathfinder.grid.IsPositionWalkable(nextPosition, tankSize, transform.rotation))
            {
                // Rotation vers la direction alternative
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                
                // Déplacement dans cette direction
                transform.position = nextPosition;
                return;
            }
        }
        
        // Si aucune direction n'est accessible, forcer une mise à jour du chemin
        nextPathUpdate = 0;
    }

    // Vérifie si le tank est bloqué
    void CheckIfStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        
        if (distanceMoved < stuckThreshold)
        {
            stuckTime += Time.deltaTime;
            
            if (stuckTime > maxStuckTime && !isStuck && !isUnstucking)
            {
                isStuck = true;
                
                // Démarrer une tentative de déblocage
                StartUnstuck();
            }
        }
        else
        {
            // Le tank a bougé, réinitialiser les compteurs
            stuckTime = 0f;
            isStuck = false;
        }
        
        // Mettre à jour la dernière position
        lastPosition = transform.position;
    }
    
    // Démarre une tentative de déblocage
    void StartUnstuck()
    {
        // Choisir une direction aléatoire pour se débloquer
        float randomAngle = Random.Range(0f, 360f);
        unstuckDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
        
        // Démarrer l'action de déblocage
        isUnstucking = true;
        unstuckTimer = 0f;
        
        // Forcer une mise à jour du chemin après la tentative de déblocage
        nextPathUpdate = Time.time + unstuckActionDuration + 0.5f;
    }
    
    // Continue l'action de déblocage en cours
    void ContinueUnstuckAction()
    {
        unstuckTimer += Time.deltaTime;
        
        // Déplacer le tank dans la direction choisie
        if (unstuckTimer <= unstuckActionDuration)
        {
            // Calculer la position cible
            Vector3 targetPosition = transform.position + unstuckDirection * unstuckMoveDistance;
            targetPosition.y = initialY;
            
            // Déplacer progressivement vers la position cible
            float progress = unstuckTimer / unstuckActionDuration;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, progress);
            
            // Vérifier si la position est accessible
            if (pathfinder.grid.IsPositionWalkable(newPosition, tankSize, transform.rotation))
            {
                transform.position = newPosition;
            }
        }
        
        // Terminer l'action de déblocage après la durée prévue
        if (unstuckTimer >= unstuckActionDuration)
        {
            isUnstucking = false;
            
            // Vérifier si le tank a réussi à se débloquer
            if (Vector3.Distance(transform.position, lastPosition) > stuckThreshold * 2)
            {
                isStuck = false;
                stuckTime = 0f;
            }
        }
    }
} 