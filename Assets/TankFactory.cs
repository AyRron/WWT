using UnityEngine;

using UnityEngine;

public class TankFactory : MonoBehaviour
{
    public GameObject playerTankPrefab;
    public GameObject enemyTankPrefab;

    public GameObject CreateTank(string type, Vector3 position, Quaternion rotation)
    {
        GameObject prefabToSpawn = null;

        if (type == "Player")
            prefabToSpawn = playerTankPrefab;
        else if (type == "Enemy")
            prefabToSpawn = enemyTankPrefab;
        else
        {
            Debug.LogError($"Type de tank inconnu : {type}");
            return null;
        }

        GameObject tank = Instantiate(prefabToSpawn, position, rotation);
        tank.tag = type;
        return tank;
    }
}

