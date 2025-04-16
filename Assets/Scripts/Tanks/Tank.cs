using UnityEngine;

public class Tank : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int id;
    void Start()
    {
        TankSelectionManager.Instance.allTanksList.Add(gameObject);

    }

    private void OnDestroy()
    {
        if (TankSelectionManager.Instance != null)
        {
            TankSelectionManager.Instance.allTanksList.Remove(gameObject);
        }
    }

}
