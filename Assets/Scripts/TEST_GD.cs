using NaughtyAttributes;
using UnityEngine;

public class TEST_GD : MonoBehaviour
{
    public static TEST_GD Instance;
    private void Awake() { Instance = this; }

    [BoxGroup("Références de scripts (ne pas retirer)")]
    [SerializeField] private DominoMovementController dominoMvtController;
    [BoxGroup("Références de scripts (ne pas retirer)")]
    [SerializeField] private DominoSpawner spawner;

    [Space(5)]

    [BoxGroup("-- Spawn d'un domino --")][Header("Activer la _currentRotation aléatoire")]
    public bool RotationRandom;

    [BoxGroup("-- deplacement d'un domino --")]
    [Header("Activer la tombée case par case")]
    public bool FallPerCase;
    [BoxGroup("-- deplacement d'un domino --"), EnableIf("FallPerCase"), Header("temps entre chaque step en secondes")]
    public float FallingStepStoppingTime = 1f;


    [Button]
    private void RespawnUnAutreDomino() 
    { 
        if (dominoMvtController != null)
        {
            if (dominoMvtController.CurrentDomino != null)
            {
                Destroy(dominoMvtController.CurrentDomino.gameObject);

                if (spawner != null)
                    spawner.SpawnNextDomino();
            }
            
        }
    }

    [BoxGroup("-- INFOS --"), ReadOnly, ResizableTextArea]
    public string autre = "Modifier infos sur le domino -> objet DominoMouvementController";
    [BoxGroup("-- INFOS --"), ReadOnly, ResizableTextArea]
    public string autre1 = "Modifier infos de la grille -> objet GridManager";
}
