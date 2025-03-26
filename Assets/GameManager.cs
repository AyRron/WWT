using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public List<Tank> tanksAllies = new List<Tank>();
    public List<Tank> tanksEnnemies = new List<Tank>();

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
}
