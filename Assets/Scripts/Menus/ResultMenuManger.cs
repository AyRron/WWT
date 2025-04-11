using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMenuManager : MonoBehaviour
{
    public GameObject victoryScreen;
    public GameObject defeatScreen;
    
    private AudioSource _audioSource;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();

        if (CrossSceneInformations.Victory)
        {
            victoryScreen.SetActive(true);
            _audioSource.clip = victoryMusic;
        }
        else
        {
            defeatScreen.SetActive(true);
            _audioSource.clip = defeatMusic;
        }
        
        _audioSource.Play();
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
