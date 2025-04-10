using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMenuManager : MonoBehaviour
{
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    private void Start()
    {
        if (CrossSceneInformations.Victory)
        {
            victoryScreen.SetActive(true);
        }
        else
        {
            defeatScreen.SetActive(true);
        }
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene ("GameScene");
    }
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene ("MainMenu");
    }
}
