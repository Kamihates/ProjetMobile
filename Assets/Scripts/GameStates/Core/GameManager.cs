using UnityEngine;
using NaughtyAttributes;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField, Foldout("Config"), Required] private GameConfig gameConfig;
    [SerializeField, Foldout("Config")] private GameState defaultState = GameState.TitleScreenState;

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

    //public Action OnGameLost; // action quand un domino est placé en haut de la grille
    //public Action OnWin;
    public Action<GameState> OnStateChanged; // Nouvelle action principale pour les gérer les states 
    public Action OnInfiniteGameStarted;
    public Action OnCurrentDominoChanged;

    private void Awake() { Instance = this; _isInInfiniteState = gameConfig.LoopAfterBoss; _noGravityMode = gameConfig.NoGravityMode; }

    private void Start()
    {
        ChangeState(defaultState); // On change la scene par defaut au lancement du jeu (par default c'est le splash screen)

    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    public void StartGame()
    {
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
        ChangeState(GameState.LoseState);
        CurrentDomino = null;
        //Pause(true);
    }

    public void GameWon()
    {
        //OnWin?.Invoke();
        ChangeState(GameState.WinState);
        Pause(true);
    }

    public void ReturnToGame()
    {
        Time.timeScale = 1;
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
        ChangeState(GameState.MenuState);
    }

    public void GoToSettings()
    {
        ChangeState(GameState.SettingsState);
    }

    public void DisableAutoFall(bool activate)
    {
        gameConfig.NoGravityMode = activate;
    }

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
}