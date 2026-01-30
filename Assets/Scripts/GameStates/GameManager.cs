using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private DominoSpawner dominoSpawner;

    private void Start()
    {
        deckManager.OnSpawnDomino += HandleNextDomino;

        deckManager.SpawnNextDomino();
    }

    private void HandleNextDomino(DominoData dominoData)
    {
        Domino domino = dominoSpawner.SpawnDomino(dominoData);
    }

    private void OnDestroy()
    {
        if (deckManager != null)
            deckManager.OnSpawnDomino -= HandleNextDomino;
    }
}
