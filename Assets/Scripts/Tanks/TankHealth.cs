using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class TankHealth : MonoBehaviour
{
    public float startingHealth = 100f;               // The amount of health each tank starts with.
    public Slider slider;                             // The slider to represent how much health the tank currently has.
    public Color fullHealthColor = Color.green;       // The color the health bar will be when on full health.
    public Color zeroHealthColor = Color.red;         // The color the health bar will be when on no health.


    public float _currentHealth;                      // How much health the tank currently has.
    private bool _dead;                                // Has the tank been reduced beyond zero health yet?


    private void Start()
    {
        if (slider == null)
        {
            slider = GetComponentInChildren<Slider>();
            slider.minValue = 0f;
            slider.maxValue = startingHealth;
            slider.value = startingHealth;
        }

        if (slider == null)
        {
            Debug.LogError("Le Slider n'a pas été trouvé dans les enfants de " + gameObject.name);
        }

        SetHealthUI();
    }


    private void OnEnable()
    {
        _currentHealth = startingHealth;
        _dead = false;
        

        SetHealthUI();
    }


    public void TakeDamage(float amount)
    {
        Debug.Log("Touché");
        if(gameObject.CompareTag("Enemy")){
            Debug.Log("Touché enemy");
        }
        _currentHealth -= amount;

        SetHealthUI();

        if (_currentHealth <= 0f && !_dead)
        {
            OnDeath();
        }
    }


    private void SetHealthUI()
    {
        slider.value = _currentHealth;
        var fillImage = slider.fillRect.GetComponent<Image>();
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, _currentHealth / startingHealth);
    }


    private void OnDeath()
    {
        _dead = true;
        TankSpawnerManager.Instance.RespawnTank(gameObject, 5f);
        gameObject.SetActive(false);
    }

}
