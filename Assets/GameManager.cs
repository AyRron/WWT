using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    private List<Tank> tanksAllies;
    private List<Tank> tanksEnnemies;

    public float scoreAllies = 0f;
    public float scoreEnnemies = 0f;

    // Awake is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Tank> GetTanksAllies()
    {
        return tanksAllies;
    }

    public List<Tank> GetTankEnnemies()
    {
        return tanksEnnemies;
    }
}
