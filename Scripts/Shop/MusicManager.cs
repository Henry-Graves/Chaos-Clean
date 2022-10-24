using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to fade in and out different songs based on entering/exiting a collider trigger.
// Built such that the songs do not stop or pause, just get muted.
public class MusicManager : MonoBehaviour
{
    public AudioClip fightMusicClip;
    public AudioClip shopMusicClip;
    [Range(0,1)] public float fightVolume;
    [Range(0,1)] public float shopVolume;
    [Range(0,1)] public float FadeRate;

    private AudioSource fightMusic;
    private AudioSource shopMusic;

    void Start()
    {
        // Initialize audiosources and music, play fight music
        fightMusic = gameObject.AddComponent<AudioSource>();
        shopMusic = gameObject.AddComponent<AudioSource>();

        shopMusic.volume = shopVolume;
        shopMusic.clip = shopMusicClip;
        shopMusic.loop = true;

        fightMusic.volume = fightVolume;
        fightMusic.clip = fightMusicClip;
        fightMusic.loop = true;
        fightMusic.Play();
    }

    // If player enters the defined zone for the shop...
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Quickly fade the fight music volume to 0. Don't stop the song or reset its progress.
            StartCoroutine(FadeOut(fightMusic));

            // Play the shop music if not already started.
            if (!shopMusic.isPlaying)
                shopMusic.Play();
            else
                StartCoroutine(FadeIn(shopMusic, shopVolume));
        }
    }

    // If player exits the defined zone for the shop...
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // Quickly fade the shop music volume to 0. Don't stop the song or reset its progress.
            StartCoroutine(FadeOut(shopMusic));

            // Quickly fade in the fight music
            StartCoroutine(FadeIn(fightMusic, fightVolume));
        }
    }

    // Fade out to 0
    private IEnumerator FadeOut(AudioSource audioSource)
    {
        // Get current volume
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeRate;
            yield return null;
        }
    }

    // Fade in up to target volume
    private IEnumerator FadeIn(AudioSource audioSource, float targetVolume)
    {
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / FadeRate;
            yield return null;
        }
    }

}
