using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public List<Tank> tanksAllies = new List<Tank>();
    public List<Tank> tanksEnemies = new List<Tank>();

    public Image alliesScore;
    public Image enemiesScore;

    public float scoreAllies;
    public float scoreEnemies;

    public GameObject timerUI;
    private TextMeshProUGUI _timerText;
    private float _timer = 120f;
    
    private void Awake()
    {
        if (timerUI != null)
        {
            _timerText = timerUI.GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScoreBare();
        UpdateTimer();
    }

    private void UpdateScoreBare()
    {
        alliesScore.fillAmount = scoreAllies / 100f;
        enemiesScore.fillAmount = scoreEnemies / 100f;
    }
    
    private void UpdateTimer()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            var minutes = Mathf.FloorToInt(_timer / 60);
            var seconds = Mathf.FloorToInt(_timer % 60);
            _timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            _timer = 0;
            _timerText.text = "00:00";
            EndGame();
        }
    }

    
    private void EndGame()
    {
        SceneManager.LoadScene ("MainMenu");
    }
}
