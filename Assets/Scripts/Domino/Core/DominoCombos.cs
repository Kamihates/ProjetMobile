using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System;
using GooglePlayGames;
using System.Linq;

public class DominoCombos : MonoBehaviour
{
    [SerializeField] private float damagePerCombo = 5;
    public float DamagePerCombo => damagePerCombo;
    [SerializeField, Label("un t1 basique multiplie par combien ? (basique = 4)")] private float T1multipicator = 2;
    public float T1Multipicator => T1multipicator;


    [SerializeField] private DominoFusion dominoFusion;

    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentDomino;
    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentR1;
    [SerializeField, Foldout("Debug"), ReadOnly] private List<Vector2Int> combosOfAdjacentR2;

    public Action<float> OnComboDamage;
    public Action<List<Vector2Int>, float> OnComboChain;
    public Action<float, float, bool, bool> OnComboFinished;

    public Dictionary<RegionType, bool> _hascomboOf4 = new Dictionary<RegionType, bool>();

    [SerializeField] private BossController _bossController;


    private float _TotalDamageCounter = 0;

    private void Start()
    {
        GridManager.Instance.OnDominoPlaced += CheckForReaction;

        resetCounters();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnInfiniteGameStarted += resetCounters;
        }

    }

    private void OnDestroy()
    {
        if (GridManager.Instance != null) 
            GridManager.Instance.OnDominoPlaced -= CheckForReaction;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnInfiniteGameStarted -= resetCounters;
        }
    }

    private void resetCounters()
    {
        _hascomboOf4[RegionType.Fire] = false;
        _hascomboOf4[RegionType.Wind] = false;
        _hascomboOf4[RegionType.Rock] = false;
        _hascomboOf4[RegionType.Water] = false;
    }


    public void CheckForReaction(DominoPiece piece)
    {
        // d'abord on check les combos.
        // si ya une ou des fusions, on calcule leurs bonus pour les ajouter aux degats

        float comboDamages = CheckForCombos(piece);
        float fusionBonusDamage = dominoFusion.CheckForFusion(piece);

        float totalDamage = comboDamages + fusionBonusDamage;

        if (totalDamage >= 25)
        {
            // succes "Critical Spell"
            PlayGamesPlatform.Instance.ReportProgress("CgkIjP3qhoIaEAIQCQ", 100.0f, (bool success) =>
            {
                if (success)
                    Debug.Log("Succ�s d�bloqu� !");
                else
                    Debug.Log("�chec du d�blocage du succ�s.");
            });
        }

        OnComboDamage?.Invoke(totalDamage);
    }

    public float CheckForCombos(DominoPiece piece)
    {

        float combosOfAdjacentR1 = 0;
        float combosOfAdjacentR2 = 0;

        RegionPiece regionPiece1 = piece.transform.GetChild(0).GetComponent<RegionPiece>();
        RegionPiece regionPiece2 = piece.transform.GetChild(1).GetComponent<RegionPiece>();

        if (regionPiece1.gameObject.activeSelf && regionPiece1.Region != null)
            combosOfAdjacentR1 = CheckForAdjacentDomino(regionPiece1);

        if (regionPiece2.gameObject.activeSelf
            && regionPiece2.Region != null
            && (!regionPiece1.gameObject.activeSelf
                || regionPiece1.Region.RegionID != regionPiece2.Region.RegionID))
        {
            combosOfAdjacentR2 = CheckForAdjacentDomino(regionPiece2);
        }


        float totalDamage = combosOfAdjacentR1 + combosOfAdjacentR2;
            
        if (GameManager.Instance.IsInfiniteState)
            _TotalDamageCounter += totalDamage;

        return totalDamage;
    }


    private float CheckForAdjacentDomino(RegionPiece regionPiece)
    {
        combosOfAdjacentDomino.Clear();
        int t1Count = 0;
        // Donne l'index par rapport a la grille
        Vector2Int regionIndex = GridManager.Instance.GetIndexFromPosition(regionPiece.transform.position);

        List<Vector2Int> regionToCheck = new List<Vector2Int> { regionIndex };

        while(regionToCheck.Count> 0)
        {
            Vector2Int currentIndex = regionToCheck[0];
            regionToCheck.RemoveAt(0);

            RegionData regionData = GridManager.Instance.GetRegionAtIndex(currentIndex)?.Region;
            if (regionData == null)
                continue;

            if (!combosOfAdjacentDomino.Contains(currentIndex))
            {
                combosOfAdjacentDomino.Add(currentIndex);

                if (GridManager.Instance.GetRegionAtIndex(currentIndex).DominoParent.Data.IsDominoFusion)
                {
                    t1Count++;
                }
            }

            Vector2Int[] regionNeighbors  = GetRegionNeighbors(currentIndex);

            foreach (Vector2Int neighbor in regionNeighbors)
            {
                if (!GridManager.Instance.CheckIndexValidation(neighbor))
                    continue;

                RegionData neighborRegion = GridManager.Instance.GetRegionAtIndex(neighbor)?.Region;

                if (neighborRegion == null)
                    continue;

                if (neighborRegion.Type == regionPiece.Region.Type)
                {
                    if (!combosOfAdjacentDomino.Contains(neighbor))
                    {
                        regionToCheck.Add(neighbor);
                    }
                }
            }
        }

        

        // pour le succes : avec une combo d'au moins 4 piece de tout les types
        if (combosOfAdjacentDomino.Count >= 4)
        {
            _hascomboOf4[regionPiece.Region.Type] = true;


            bool allTrue = _hascomboOf4.Values.All(v => v);

            if (allTrue)
            {
                // succes "Arcane Harmony"
                PlayGamesPlatform.Instance.ReportProgress("CgkIjP3qhoIaEAIQBA", 100.0f, (bool success) =>
                {
                    if (success)
                        Debug.Log("Succ�s d�bloqu� !");
                    else
                        Debug.Log("�chec du d�blocage du succ�s.");
                });
            }



        }

        if (combosOfAdjacentDomino.Count >= 8 && !GameManager.Instance.IsInfiniteState)
        {
            // succes "major convergence"
            PlayGamesPlatform.Instance.ReportProgress("CgkIjP3qhoIaEAIQAw", 100.0f, (bool success) =>
            {
                if (success)
                    Debug.Log("Succ�s d�bloqu� !");
                else
                    Debug.Log("�chec du d�blocage du succ�s.");
            });
        }


        // application des degats totaux de la region

        float comboDmg = combosOfAdjacentDomino.Count;


        if (comboDmg == 1) comboDmg = 0;

        comboDmg *= damagePerCombo; // application des degat par piece



        if (t1Count / 2 > 0)
        {
            comboDmg *= ((t1Count / 2) * T1Multipicator);
        }

        bool isWeakness = false;
        bool isResistance = false;


        // calcule selon les resistances
        if (_bossController.Resistance == regionPiece.Region.Type)
        {
            // si il est resistant / 2
            comboDmg /= 2;
            isResistance = true;
        }
        if (_bossController.Weakness == regionPiece.Region.Type)
        {
            comboDmg *= 1.5f;
            isWeakness = true;
        }

        if (comboDmg > 0)
            OnComboFinished?.Invoke(comboDmg, T1Multipicator, isWeakness, isResistance);

        if (combosOfAdjacentDomino.Count >= 2)
        {
            OnComboChain?.Invoke(new List<Vector2Int>(combosOfAdjacentDomino), damagePerCombo);
        }
        return (comboDmg);
    }

    private Vector2Int[] GetRegionNeighbors(Vector2Int regionIndex)
    {
        Vector2Int[] neighbors = new Vector2Int[]
        {
            new Vector2Int(0, 1) + regionIndex,   // Up
            new Vector2Int(1, 0) + regionIndex,   // Right
            new Vector2Int(0, -1) + regionIndex,  // Down
            new Vector2Int(-1, 0) + regionIndex  // Left
        };

        return neighbors;
    }
}
