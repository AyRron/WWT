using UnityEngine;
using System.Collections.Generic;

public class TankSelectionCameraManager : MonoBehaviour
{
    public static TankSelectionCameraManager Instance;

    public List<GameObject> allTanksList = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Invoke(nameof(CollectAllTanks), 0.1f); // petit délai pour laisser les tanks se spawn
    }

    private void CollectAllTanks()
    {
        GameObject[] foundTanks = GameObject.FindGameObjectsWithTag("Player");
        allTanksList.Clear();
        allTanksList.AddRange(foundTanks);

        Debug.Log($"[TankSelectionCameraManager] {allTanksList.Count} tanks trouvés dans la scène.");
    }


    // Récupérer un tank par ID
    public GameObject GetTankById(int id)
    {
        foreach (var tank in allTanksList)
        {
            Tank tankScript = tank.GetComponent<Tank>();
            if (tankScript != null && tankScript.id == id)
            {
                return tank;
            }
        }
        return null;
    }
}
