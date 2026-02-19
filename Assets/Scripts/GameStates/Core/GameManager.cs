using GooglePlayGames;
using NaughtyAttributes;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField, Foldout("Config"), Required] private GameConfig gameConfig;
    [SerializeField, Foldout("Config")] private GameState defaultState = GameState.TitleScreenState;

    [SerializeField, Foldout("ToggleSettings"), Required] private ToggleSwitch noGravityToggle;
    //[SerializeField, Foldout("ToggleSettings"), Required] private ToggleSwitch fallPerCaseToggle;

    [SerializeField, Foldout("Références"), Required] private DeckManager deckManager;
    [SerializeField, Foldout("Références"), Required] private DominoSpawner dominoSpawner;
    [SerializeField, Foldout("Références"), Required] private DominoMovementController dominoMouvement;

    [field: SerializeField, Foldout("Debug"), ReadOnly] public GameState CurrentState { get; private set; }
    [SerializeField, Foldout("Debug"), ReadOnly] private int currentRound = 0;
    [SerializeField, Foldout("Debug"), ReadOnly] private bool isBossRound = false;

    private bool _isInInfiniteState = false;
    private bool _noGravityMode = false;
    public bool NoGravityMode => _noGravityMode;
    public bool IsInfiniteState => _isInInfiniteState;
    public DominoPiece CurrentDomino { get => _currentDomino; set { _currentDomino = value; dominoMouvement.CurrentDomino = value; OnCurrentDominoChanged?.Invoke(); } }

    private DominoPiece _currentDomino;

    //public TextMeshProUGUI _testAchievement;

    //public Action OnGameLost; // action quand un domino est placé en haut de la grille
    //public Action OnWin;
    public Action<GameState> OnStateChanged; // Nouvelle action principale pour les gérer les states 
    public Action OnInfiniteGameStarted;
    public Action OnCurrentDominoChanged;

    private void Awake()
    {
        Instance = this;
        _isInInfiniteState = gameConfig.LoopAfterBoss;
        _noGravityMode = gameConfig.NoGravityMode;

        if (PlayerPrefs.HasKey("NoGravityMode"))
            _noGravityMode = PlayerPrefs.GetInt("NoGravityMode") == 1;
        else
            _noGravityMode = gameConfig.NoGravityMode;

        //if (PlayerPrefs.HasKey("FallPerCase"))
        //    gameConfig.FallPerCase = PlayerPrefs.GetInt("FallPerCase") == 1;

    }

    private void Start()
    {
        Debug.Log("current scene index = " + SceneManager.GetActiveScene().buildIndex);

        if (gameConfig.SkipTitleScreens && SceneManager.GetActiveScene().buildIndex == 0)
            defaultState = GameState.MenuState;

        ChangeState(defaultState); // On change la scene par defaut au lancement du jeu (par default c'est le splash screen)

        //fallPerCaseToggle.SetInitialState(gameConfig.FallPerCase);
        noGravityToggle.SetInitialState(gameConfig.NoGravityMode);  
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    public void OnApplicationQuit()
    {
        gameConfig.SkipTitleScreens = false;
    }

    public void StartGame()
    {
        gameConfig.SkipTitleScreens = true;
        Pause(false);
        gameConfig.LoopAfterBoss = false;
        SceneManager.LoadSceneAsync(1);
        ChangeState(GameState.InGameState);
        currentRound = 0;
        isBossRound = false;
        _isInInfiniteState = gameConfig.LoopAfterBoss;

    }

    public void OnMobDefeated()
    {
        currentRound++;

        if (isBossRound)
        {
            isBossRound = false;

            if (gameConfig.LoopAfterBoss)
            {
                currentRound = 0;
                ChangeState(GameState.InGameState);
            }
            else
                ChangeState(GameState.WinState);
        }


        ChangeState(GameState.InGameState);
        Pause(false);
    }

    public void GameLost()
    {
        //OnGameLost?.Invoke();
        StatsVisual.Instance.DisplayStatsOnDeath();
        ChangeState(GameState.LoseState);
        CurrentDomino = null;
        //Pause(true);
    }

    public void GameWon()
    {
        //OnWin?.Invoke();
        StatsVisual.Instance.DisplayStatsOnWin();
        ChangeState(GameState.WinState);
        Pause(true);
    }

    public void ReturnToGame()
    {
        Pause(false);
        ChangeState(GameState.InGameState);
    }

    public void Tuto()
    {
        ChangeState(GameState.TutoState);
        Pause(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
        ChangeState(GameState.InGameState);
        Pause(false);
    }

    public void GoToMenu()
    {
        Pause(false);
        ChangeState(GameState.MenuState);
    }

    public void GoToCredits()
    {
        ChangeState(GameState.CreditsState);
        Pause(true);
    }

    public void GoToSettings()
    {
        ChangeState(GameState.SettingsState);
        Pause(true);
    }

    public void DisableAutoFall(bool activate)
    {
        gameConfig.NoGravityMode = activate;
        _noGravityMode = activate;

        PlayerPrefs.SetInt("NoGravityMode", activate ? 1 : 0);
        PlayerPrefs.Save();
    }


    //public void EnableFallPerCase(bool activate)
    //{
    //    gameConfig.FallPerCase = activate;

    //    PlayerPrefs.SetInt("FallPerCase", activate ? 1 : 0);
    //    PlayerPrefs.Save();

    //}

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
        ChangeState(GameState.MenuState);
    }


    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }

    public void PauseGame(bool pause)
    {
        Pause(true);
        ChangeState(pause ? GameState.PauseState : GameState.InGameState);
    }

    public void GoInInfiniteState()
    {
        Time.timeScale = 1;
        gameConfig.LoopAfterBoss = true;
        SceneManager.LoadScene(1);

    }

    //public void ShowAchievements()
    //{
    //    //PlayGamesPlatform.Instance.ShowAchievementsUI();
    //    Debug.Log("AUTH BEFORE UI = " + PlayGamesPlatform.Instance.localUser.authenticated); 

    //    if (PlayGamesPlatform.Instance.localUser.authenticated)
    //        PlayGamesPlatform.Instance.ShowAchievementsUI(); 
    //    else 
    //        Debug.Log("Impossible d'afficher les succès : non authentifié.");
    //}

    ////public void Unlock()
    ////{
    ////    if (PlayGamesPlatform.Instance.localUser.authenticated)
    ////        _testAchievement.text = "authentifié / " + PlayGamesPlatform.Instance.localUser.userName;
    ////    else
    ////    {
    ////        _testAchievement.text = "pas authentifié / ";
    ////        PlayGamesPlatform.Instance.Authenticate(authSuccess => Debug.Log("authentification..."));
    ////    }


    ////    PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_unleashed_power, 100.0f, (bool success) =>
    ////    {
    ////        if (success)
    ////            Debug.Log("succès dévérouillé");
    ////        else
    ////            Debug.Log("échec du déblocage du succès.");

    ////        _testAchievement.text += success ? " Succès débloqué !" : " echec..";

    ////    });
    ////}

}