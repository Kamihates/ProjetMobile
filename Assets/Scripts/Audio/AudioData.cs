using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

// Classe pour associer une musique à une scène
[System.Serializable]
public class SceneMusic
{
    [SerializeField, Scene] private string sceneName; // Nom de la scène
    public string SceneName => sceneName;
    [SerializeField]  private AudioClip musicClip; // Clip de musique associé
    public AudioClip MusicClip => musicClip;
}

// Classe pour associer un son à un objet trigger
[System.Serializable]
public class TriggerSound
{
    [SerializeField] private string objectName; // Nom de l'objet trigger
    public string ObjectName => objectName;
    [SerializeField] private AudioClip soundClip; // Son associé à l'objet
    public AudioClip SoundClip => soundClip;
}

[System.Serializable]
public class AudioData
{

    [Foldout("Musiques de scènes"), SerializeField] private List<SceneMusic> sceneMusics = new();
    public List<SceneMusic> SceneMusics => sceneMusics;

    [Foldout("Sons de triggers"), SerializeField] private List<TriggerSound> triggerSounds = new();
    public List<TriggerSound> TriggerSounds => triggerSounds;

    [Header("Autres sons"), HorizontalLine(color:EColor.Blue)]
    [SerializeField] private AudioClip comboSound;
    [SerializeField] private AudioClip dominoPlaced;
    [SerializeField] private AudioClip dominoDash;
    [SerializeField, HorizontalLine(color: EColor.Blue)] private AudioClip waterFusion;
    [SerializeField] private AudioClip earthFusion;
    [SerializeField] private AudioClip fireFusion;
    [SerializeField] private AudioClip windFusion;

    #region Propriétes

    public AudioClip ComboSound => comboSound;
    public AudioClip DominoPlaced => dominoPlaced;
    public AudioClip DominoDash => dominoDash;
    public AudioClip WaterFusion => waterFusion;
    public AudioClip FireFusion => fireFusion;

    public AudioClip EarthFusion => earthFusion;
    public AudioClip WindFusion => windFusion;

    #endregion

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
