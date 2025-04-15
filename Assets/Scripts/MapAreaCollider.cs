using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MapAreaCollider : MonoBehaviour
{

    private List<Tank> playerAreaList = new List<Tank>();

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Something has trigger the area");
        if (collider.TryGetComponent<TankColliderZone>(out TankColliderZone playerArea))
        {
            playerAreaList.Add(playerArea.GetComponentInParent<Tank>());
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.TryGetComponent<TankColliderZone>(out TankColliderZone playerArea))
        {
            playerAreaList.Remove(playerArea.GetComponentInParent<Tank>());
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
