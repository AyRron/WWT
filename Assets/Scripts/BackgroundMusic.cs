using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip backgroundMusic;
    public float fadeDuration = 1.5f;

    private void Start()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.clip = backgroundMusic;
        _audioSource.loop = true;
        _audioSource.volume = 0f;
        _audioSource.Play();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        var t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        var startVolume = _audioSource.volume;
        var t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        _audioSource.Stop();
    }
}
