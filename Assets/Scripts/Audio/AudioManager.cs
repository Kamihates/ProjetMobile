using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using NaughtyAttributes;

[System.Serializable]
public class VolumeControl
{
    [SerializeField, Foldout("AudioMixer")] private string exposedParameter;
    [SerializeField, Foldout("AudioMixer")] private AudioMixerGroup mixerGroup;
    [SerializeField, Foldout("AudioMixer")] private Slider slider;
    [SerializeField, Foldout("Mute icon")] private Image icon;
    [SerializeField, Foldout("Mute icon")] private Sprite onVolumeSprite;
    [SerializeField, Foldout("Mute icon")] private Sprite offVolumeSprite;

    [SerializeField, Foldout("Debug"), ReadOnly] private float lastVolume = 1f;
    [SerializeField, Foldout("Debug"), ReadOnly] private bool isMuted = false;

    public bool IsMuted => isMuted;
    public float LastVolume => lastVolume;

    public string ExposedParameter => exposedParameter;
    public AudioMixerGroup MixerGroup => mixerGroup;

    public void SetVolume(float value)
    { 
        lastVolume = value;

        if(value <= 0f)
        {
            mixerGroup.audioMixer.SetFloat(exposedParameter, -80f);
            isMuted = true;

            if(icon != null) icon.sprite = offVolumeSprite;
        }
        else
        {
            mixerGroup.audioMixer.SetFloat(exposedParameter, Mathf.Log10(value) * 20);
            isMuted = false;

            if(icon != null)
                icon.sprite = onVolumeSprite;
        }    

        if(slider != null)
            slider.value = value;
        PlayerPrefs.SetFloat(exposedParameter, value);
    }

    public void ToggleMute()
    {
        if(isMuted)
            SetVolume(lastVolume);
        else
        {
            lastVolume = slider != null ? slider.value : lastVolume;
            mixerGroup.audioMixer.SetFloat(exposedParameter, -80f);
            isMuted = true;
            if (icon != null) icon.sprite = offVolumeSprite;
            if (slider != null) slider.value = 0f;
        }
    }
}

public class AudioManager : MonoBehaviour
{
    // Singleton pour pouvoir accéder à l'AudioManager depuis n'importe où
    public static AudioManager Instance;

    [SerializeField, Foldout("Setting"), Range(0f, 1f)] private float defaultVolume = 0.5f;
    [SerializeField, Foldout("Silder")] private VolumeControl masterSlider;
    [SerializeField, Foldout("Silder")] private VolumeControl musicSlider;
    [SerializeField, Foldout("Silder")] private VolumeControl sfxSlider;

    [SerializeField] private AudioData audioData;
    public AudioData DataAudio => audioData;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private readonly HashSet<string> playedSounds = new(); // Liste des sons déjà joués pour chaque objet unique (identifiant unique par objet)

    private AudioClip previousMusic; // pour sauvegarder la musique avant le boost
    private bool isBoostMusic = false;


    private void Awake()
    {
        // Check si un AudioManager existe déjà dans la scène
        if (Instance != null && Instance != this)
        {
            Instance.Cleanup(); // Se désabonne de l'event
            Destroy(Instance.gameObject); // Détruit l'ancien proprement
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject); // Ne pas détruire l'objet quand la scène change
        InitSources(); // Initialisation des sources audio
        SceneManager.sceneLoaded += OnSceneLoaded; // Appel de la méthode OnSceneLoaded quand la scène est chargée
    }

    private void Start()
    {
        masterSlider.SetVolume(PlayerPrefs.GetFloat(masterSlider.ExposedParameter, defaultVolume));
        musicSlider.SetVolume(PlayerPrefs.GetFloat(musicSlider.ExposedParameter, defaultVolume));
        sfxSlider.SetVolume(PlayerPrefs.GetFloat(sfxSlider.ExposedParameter, defaultVolume));
    }

    // Méthode pour initialiser les sources audio
    private void InitSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicSlider.MixerGroup;
        musicSource.loop = true; // Boucle la musique
        musicSource.playOnAwake = false; // Ne pas jouer la musique dès l'initialisation

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxSlider.MixerGroup;
        sfxSource.playOnAwake = false;
    }

    // Méthode appelée à chaque chargement de scène
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip music = audioData.GetSceneMusic(scene.name);
        if (music != null)
            PlayMusic(music); // Jouer la musique définie pour la scène
        else
            StopMusic(); // Arrêter la musique si aucune n'est définie pour la scène
    }

    // Jouer une musique spécifique
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null) return;

        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
        }

        musicSource.Play(); 
    }

    public void PlayTemporaryMusic(AudioClip tempClip, float duration)
    {
        if (musicSource.clip == tempClip) return;

        previousMusic = musicSource.clip;
        isBoostMusic = true;
        PlayMusic(tempClip);

        // Revenir à la musique précédente après le délai
        StartCoroutine(BackToPreviousMusic(duration));
    }

    private IEnumerator BackToPreviousMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (previousMusic != null && isBoostMusic)
        {
            PlayMusic(previousMusic);
            isBoostMusic = false;
        }
    }

    public void StopMusic() => musicSource.Stop();
    public void StopSFX() => sfxSource.Stop();

    // Jouer un effet sonore
    public void PlaySFX(AudioClip clip) => sfxSource.PlayOneShot(clip); // Jouer un effet sonore une fois

    // Jouer le son d'un trigger (objet interactif)
    public void PlayTriggerSound(string objectName, string uniqueId)
    {
        if (playedSounds.Contains(uniqueId)) return;

        AudioClip clip = audioData.GetTriggerSound(objectName);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
            playedSounds.Add(uniqueId);
        }
    }
    
    public void OnMasterSliderUpdated(float value) => masterSlider.SetVolume(value);
    public void OnMusicSliderUpdated(float value) => musicSlider.SetVolume(value);
    public void OnSFXSliderUpdated(float value) => sfxSlider.SetVolume(value);

    public void OnMasterMuteClicked() => masterSlider.ToggleMute();
    public void OnMusicMuteClicked() => musicSlider.ToggleMute();
    public void OnSFXMuteClicked() => sfxSlider.ToggleMute();

    // Cleanup appelé si une instance est détruite
    private void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
