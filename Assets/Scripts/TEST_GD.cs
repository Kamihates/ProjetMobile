using NaughtyAttributes;
using UnityEngine;

public class TEST_GD : MonoBehaviour
{
    public static TEST_GD Instance;
    private void Awake() { Instance = this; }

    [BoxGroup("Références de scripts (ne pas retirer)")]
    [SerializeField] private DominoMovementController dominoMvtController;
    [BoxGroup("Références de scripts (ne pas retirer)")]
    [SerializeField] private DeckManager deckManager;

    [BoxGroup("-- Spawn d'un domino --")][Header("Activer la rotation aléatoire")]
    public bool RotationRandom;

    [Button]
    private void Respawn() 
    { 
        if (dominoMvtController != null)
        {
            if (dominoMvtController.CurrentDomino != null)
            {
                Destroy(dominoMvtController.CurrentDomino.gameObject);

                if (deckManager!=null)
                    deckManager.SpawnNextDomino();
            }
            
        }
    }
}
