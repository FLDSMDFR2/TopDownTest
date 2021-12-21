using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public bool EnableSound = true;
    public Sound[] Sounds;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance != null)
        {
            TraceManager.WriteTrace(TraceChannel.Main, TraceType.error, "Multi AudioManager");
            return;
        }
        Instance = this;

        foreach (Sound s in Sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.Clip;

            s.source.volume = s.Volume;
            s.source.pitch = s.Pitch;
            s.source.loop = s.Loop;
        }
    }

    void Start()
    {
        Play("Theme");
    }

    public void Play(string name)
    {
        if (!EnableSound)
            return;

        Sound sound = Array.Find(Sounds, s => s.Name == name);

        if (sound == null)
            return;

        if (sound.source.isPlaying)
            return;

        sound.source.Play();

    }

    public void Stop(string name)
    {
        if (!EnableSound)
            return;

        Sound sound = Array.Find(Sounds, s => s.Name == name);

        if (sound == null)
            return;

        sound.source.Stop();
    }

    public AudioSource GetAudio(string name, GameObject obj)
    {
        if (!EnableSound)
            return null;

        Sound sound = Array.Find(Sounds, s => s.Name == name);

        if (sound == null)
            return null;

        Sound retVal = new Sound();

        retVal.source = obj.AddComponent<AudioSource>();
        retVal.source.clip = sound.Clip;

        retVal.source.volume = sound.Volume;
        retVal.source.pitch = sound.Pitch;
        retVal.source.loop = sound.Loop;

        return retVal.source;
    }
}
