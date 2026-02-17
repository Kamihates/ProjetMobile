using System.Collections.Generic;
using UnityEngine;

// Classe pour associer une musique à une scène
[System.Serializable]
public class SceneMusic
{
    private string sceneName; // Nom de la scène
    public string SceneName => sceneName;
    private AudioClip musicClip; // Clip de musique associé
    public AudioClip MusicClip => musicClip;
}

// Classe pour associer un son à un objet trigger
[System.Serializable]
public class TriggerSound
{
    private string objectName; // Nom de l'objet trigger
    public string ObjectName => objectName;
    private AudioClip soundClip; // Son associé à l'objet
    public AudioClip SoundClip => soundClip;
}

[System.Serializable]
public class AudioData
{
    [Header("Musiques de scènes")]
    private List<SceneMusic> sceneMusics = new();
    public List<SceneMusic> SceneMusics => sceneMusics;

    [Header("Sons de triggers")]
    private List<TriggerSound> triggerSounds = new();
    public List<TriggerSound> TriggerSounds => triggerSounds;

    [Header("Autres sons")]
    private AudioClip typingSound;


    public AudioClip GetSceneMusic(string sceneName)
    {
        foreach (SceneMusic music in sceneMusics)
            if (music.SceneName == sceneName)
                return music.MusicClip;
        return null;
    }

    public AudioClip GetTriggerSound(string objectName)
    {
        foreach (TriggerSound trigger in triggerSounds)
            if (trigger.ObjectName == objectName)
                return trigger.SoundClip;
        return null;
    }
}
