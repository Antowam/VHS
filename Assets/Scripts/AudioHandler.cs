using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource effectsSource;

    public AudioSource queue01;
    public AudioSource queue02;
    public AudioSource queue03;
    public AudioSource queue04;

    public static AudioHandler instance = null;

    public float lowPitchRange = .98f;
    public float highPitchRange = 1.02f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        musicSource.Play();

    }

    public void PlaySingle(AudioClip clip)
    {
        effectsSource.clip = clip;
        effectsSource.Play();
    }

    public void RandomizeEffects(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        effectsSource.pitch = randomPitch;
        effectsSource.clip = clips[randomIndex];

        if (!effectsSource.isPlaying)
            effectsSource.Play();
    }

    public void SoundQueue(AudioSource queue, params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        queue.pitch = randomPitch;
        queue.clip = clips[randomIndex];
        if(!queue.isPlaying)
            queue.Play();
    }

}
