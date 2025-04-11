using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroScreen : MonoBehaviour
{
    public float introDuration = 3f;
    public float fadeDuration = 1f;
    public Image logoImage;

    private void Start()
    {
        var color = logoImage.color;
        color.a = 0f;
        logoImage.color = color;
        
        StartCoroutine(ShowIntro());
    }

    private IEnumerator ShowIntro()
    {
        yield return new WaitForSeconds(1f);

        StartCoroutine(FadeIn());

        yield return new WaitForSeconds(introDuration);

        SceneManager.LoadScene("MainMenu");
    }
    
    private IEnumerator FadeIn()
    {
        var color = logoImage.color;

        // Animation de fondu
        var elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            logoImage.color = color;
            yield return null;  // Attendre jusqu'à la prochaine frame
        }

        color.a = 1f;
        logoImage.color = color;
    }
}