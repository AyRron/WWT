using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManger : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene ("GuiguiScene");
    }
    
    public void QuitGame()
    {
        Application.Quit ();
    }
}
