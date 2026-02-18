using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class AudioManager : MonoBehaviour
{
    // Singleton pour pouvoir accéder à l'AudioManager depuis n'importe où
    public static AudioManager Instance;

    [SerializeField, Foldout("Mixing Group")] private AudioMixerGroup masterMixer; // Groupe pour la musique
    [SerializeField, Foldout("Mixing Group")] private AudioMixerGroup musicMixer; // Groupe pour la musique
    [SerializeField, Foldout("Mixing Group")] private AudioMixerGroup sfxMixer; // Groupe pour les effets sonores

    [SerializeField, Foldout("Setting"), Range(0f, 1f)] private float defaultVolume = 0.5f;
    [SerializeField, Foldout("Silder")] private Slider masterSlider;
    [SerializeField, Foldout("Silder")] private Slider musicSlider;
    [SerializeField, Foldout("Silder")] private Slider sfxSlider;

    [SerializeField] private AudioData audioData;

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
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", defaultVolume);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", defaultVolume);

        SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        masterSlider.value = savedMaster;
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
    }

    // Méthode pour initialiser les sources audio
    private void InitSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicMixer;
        musicSource.loop = true; // Boucle la musique
        musicSource.playOnAwake = false; // Ne pas jouer la musique dès l'initialisation

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxMixer;
        sfxSource.playOnAwake = false;
    }

    // Méthode appelée à chaque chargement de scène
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var music = audioData.GetSceneMusic(scene.name);
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
        Instance.StartCoroutine(BackToPreviousMusic(duration));
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

        var clip = audioData.GetTriggerSound(objectName);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
            playedSounds.Add(uniqueId);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterMixer.audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicMixer.audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxMixer.audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
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
