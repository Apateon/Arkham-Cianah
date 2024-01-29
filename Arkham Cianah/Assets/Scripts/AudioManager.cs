using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();

            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
        }
    }

    private void Start()
    {
        PlaySound("BackgroundTheme");
    }

    public void PlaySound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        s.Source.Play();
    }

    public void StopSound(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        s.Source.Stop();
    }
}
