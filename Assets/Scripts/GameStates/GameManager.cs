using UnityEngine;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private DominoSpawner dominoSpawner;
    [SerializeField] private DominoMovementController dominoMouvement;

    public DominoPiece CurrentDomino { get => _currentDomino; set { _currentDomino = value; dominoMouvement.CurrentDomino = value; } }
    [SerializeField, Foldout("Debug"), ReadOnly] private DominoPiece _currentDomino;


    public static GameManager Instance;
    private void Awake() { Instance = this; }


}
