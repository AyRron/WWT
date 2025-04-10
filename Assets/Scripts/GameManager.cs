using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;

public class GameManager : MonoBehaviour
{
    public List<Tank> tanksAllies = new List<Tank>();
    public List<Tank> tanksEnemies = new List<Tank>();

    public Image alliesScore;
    public Image enemiesScore;

    public float scoreAllies;
    public float scoreEnemies;

    public GameObject startTimerUI;
    private TextMeshProUGUI _startTimerText;

    public GameObject timerUI;
    private TextMeshProUGUI _timerText;
    private float _timer = 12f;

    private bool _gameRunning;

    private void Awake()
    {
        if (timerUI != null)
        {
            _timerText = timerUI.GetComponent<TextMeshProUGUI>();
        }
        if (startTimerUI != null)
        {
            var text = startTimerUI.transform.Find("StartTimerText")?.gameObject;
            _startTimerText = text?.GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        StartCoroutine(CountdownBeforeStart());
    }

    private void Update()
    {
        if (!_gameRunning) return;
        
        UpdateScoreBare();
        UpdateTimer();
        
        if (scoreAllies >= 100f) StartCoroutine(EndGame(true));
        if (scoreEnemies >= 100f) StartCoroutine(EndGame(false));
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
            StartCoroutine(EndGame(false));
        }
    }

    private IEnumerator EndGame(bool victory)
    {
        _gameRunning = false;
        
        _startTimerText.text = "Terminé";
        startTimerUI.SetActive(true);

        CrossSceneInformations.Victory = victory;

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("EndGameScene");
    }

    private IEnumerator CountdownBeforeStart()
    {
        int countdown = 3;
        
        startTimerUI.SetActive(true);
        
        while (countdown > 0)
        {
            _startTimerText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        _startTimerText.text = "C'est parti!";
        yield return new WaitForSeconds(1f);
        
        startTimerUI.SetActive(false);
        _gameRunning = true;
    }
}