using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance { get; private set; }

    [SerializeField] AudioSource prefabAudioSource;

    List<AudioSource> audioSources;

    void Awake()
    {
        if (Instance) return;
        Instance = this;

        audioSources = new List<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AudioClipCoroutine(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.gameObject.SetActive(false);
    }

    public void PlaySoundEffect(AudioClip audioClip, Vector2 spawnPosition, float volume)
    {
        AudioSource audioSource = audioSources.Find(element => !element.gameObject.activeInHierarchy);
        if (!audioSource)
        {
            audioSource = Instantiate(prefabAudioSource, spawnPosition, Quaternion.identity);
            audioSources.Add(audioSource);
        }
        audioSource.gameObject.SetActive(true);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        StartCoroutine(AudioClipCoroutine(audioSource));
    }
}
