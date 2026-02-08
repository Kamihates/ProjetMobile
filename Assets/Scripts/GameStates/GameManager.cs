using UnityEngine;
using NaughtyAttributes;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private DominoSpawner dominoSpawner;
    [SerializeField] private DominoMovementController dominoMouvement;

    public DominoPiece CurrentDomino { get => _currentDomino; set { _currentDomino = value; dominoMouvement.CurrentDomino = value; } }
    
    private DominoPiece _currentDomino;

    public Action OnGameLost; // action quand un domino est placé en haut de la grille
    public Action OnWin;


    public static GameManager Instance;
    private void Awake() { Instance = this; }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }


    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }
}
