using System.Collections;
using UnityEngine;
using UnityEngine.AI; 


public class TankSpawnerManager : MonoBehaviour
{
    public static TankSpawnerManager Instance;

    private TankFactory factory;
    
    [Header("Points de spawn")]
    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;

    [Header("Nombre de tanks à générer")]
    public int numberOfPlayerTanks = 3;
    public int numberOfEnemyTanks = 3;

    void Start()
    {
        SpawnInitialTanks();
    }

    private void Awake()
    {
        
        factory = GetComponent<TankFactory>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnInitialTanks()
    {
        for (int i = 0; i < Mathf.Min(numberOfPlayerTanks, playerSpawnPoints.Length); i++)
        {
            Transform spawnPoint = playerSpawnPoints[i];
            factory.CreateTank("Player", spawnPoint.position, spawnPoint.rotation);
        }

        // Instancier les tanks ennemis
        for (int i = 0; i < Mathf.Min(numberOfEnemyTanks, enemySpawnPoints.Length); i++)
        {
            Transform spawnPoint = enemySpawnPoints[i];
            factory.CreateTank("Enemy", spawnPoint.position, spawnPoint.rotation);
        }
    }
    

    public void RespawnTank(GameObject tank, float delay)
    {
        StartCoroutine(RespawnCoroutine(tank, delay));
    }

    private IEnumerator RespawnCoroutine(GameObject tank, float delay)
    {
        yield return new WaitForSeconds(delay);

        Transform targetRespawnPoint = null;

        if (tank.CompareTag("Player") && playerSpawnPoints.Length > 0)
        {
            targetRespawnPoint = playerSpawnPoints[Random.Range(0, playerSpawnPoints.Length)];
        }
        else if (tank.CompareTag("Enemy") && enemySpawnPoints.Length > 0)
        {
            targetRespawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
        }

        if (targetRespawnPoint != null)
        {
            tank.transform.position = targetRespawnPoint.position;
            tank.transform.rotation = targetRespawnPoint.rotation;
        }

        tank.SetActive(true);

    }

}
