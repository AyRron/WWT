using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MapAreaCollider : MonoBehaviour
{

    private List<PlayerArea> playerAreaList = new List<PlayerArea>();

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<PlayerArea>(out PlayerArea playerArea))
        {
            Debug.Log("Player has enter the area");
            playerAreaList.Add(playerArea);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.TryGetComponent<PlayerArea>(out PlayerArea playerArea))
        {
            playerAreaList.Remove(playerArea);
        }
        
    }

    public List<PlayerArea> GetPlayerList()
    {
        return this.playerAreaList;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
