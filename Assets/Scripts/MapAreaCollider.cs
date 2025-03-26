using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MapAreaCollider : MonoBehaviour
{

    private List<Tank> playerAreaList = new List<Tank>();

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Something has trigger the area");
        if (collider.TryGetComponent<Tank>(out Tank playerArea))
        {
            Debug.Log("Player has enter the area");
            playerAreaList.Add(playerArea);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.TryGetComponent<Tank>(out Tank playerArea))
        {
            playerAreaList.Remove(playerArea);
        }
        
    }

    public List<Tank> GetPlayerList()
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
