using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;
using System;

public class MapArea : MonoBehaviour
{
    // State de la Area
    public enum State
    {
        Neutral,
        Captured
    }

    // Gérer l'évenement de capture de zone
    // TODO : Utiliser l'event pour gérer l'affichage
    public event EventHandler OnCaptured;

    // liste des colliders liées
    private List<MapAreaCollider> mapAreaColliderListe;
    // avancement de la capture de zone
    private float propgress;
    private float progressSpead = 1f;
    private float timeForCapture = 2f;

    // State
    private State state;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        this.mapAreaColliderListe = new List<MapAreaCollider>();


        foreach (Transform child in transform)
        {
            MapAreaCollider mapAreaCollider = child.GetComponent<MapAreaCollider>();

            if (mapAreaCollider != null)
            {
                mapAreaColliderListe.Add(mapAreaCollider);
            }
        }
        this.state = State.Neutral;

    }

    // Update is called once per frame
    void Update()
    {
        // Gérer la state
        switch (state)
        {
            // Si la zone n'est pas capturé
            case State.Neutral:
                List<PlayerArea> listePlayerAreaInside = new List<PlayerArea>();

                foreach (MapAreaCollider mapAreaCollider in mapAreaColliderListe)
                {
                    foreach (PlayerArea playerInsideArea in mapAreaCollider.GetPlayerList())
                    {
                        if (!listePlayerAreaInside.Contains(playerInsideArea))
                        {
                            listePlayerAreaInside.Add(playerInsideArea);
                        }
                    }
                }
                this.propgress += listePlayerAreaInside.Count * progressSpead * Time.deltaTime;

                Debug.Log("Player count Inside :" + listePlayerAreaInside.Count + ", progress :" + propgress);

                if(this.propgress >= this.timeForCapture)
                {
                    state = State.Captured;
                    OnCaptured?.Invoke(this, EventArgs.Empty);
                    Debug.Log("Zone capturé !");
                }

                break;
            case State.Captured:
                // Si la zone est capturé

                break;
        }

        

        
    }
}
