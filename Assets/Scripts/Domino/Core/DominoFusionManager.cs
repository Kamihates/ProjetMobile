using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DominoFusion : MonoBehaviour
{
    private GridManager _gridManager;


    private void Start()
    {
        _gridManager = GridManager.Instance;
        _gridManager.OnDominoPlaced += CheckForCombos;
    }

    private void OnDestroy()
    {
        _gridManager.OnDominoPlaced -= CheckForCombos;
    }


    private void CheckForCombos(DominoPiece piece)
    {
        // Pour chaque région du domino
        for (int i = 0; i < piece.transform.childCount; i++)
        {
            if (piece.transform.GetChild(i).TryGetComponent(out RegionPiece region))
            {
                // si la région à de la data
                if (region.Region != null)
                {
                    // on récupère les carrés de fusion
                    Vector2Int[][] fusionSquares = GetFusionNeighbors(region);
                }
            }
        }
    }

    private Vector2Int[][] GetFusionNeighbors(RegionPiece region)
    {

        List<Vector2Int> fusionNeighbors = new List<Vector2Int>();

        // On transforme notre region en index pour recup les index adjacents
        Vector2 regionPos = region.transform.position;
        Vector2Int regionIndex = _gridManager.GetPositionToGridIndex(regionPos);

        Vector2Int regionMatriceIndex = new Vector2Int(regionIndex.y, regionIndex.x); // converti index grille en index matrice

        fusionNeighbors.Add(regionMatriceIndex);

        // tableau des carrés de fusion => offsets
        Vector2Int[][] offsets = new Vector2Int[][]
        {
           
            // 0 : BL
            new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, -1),
                new Vector2Int(1, -1),
                new Vector2Int(1, 0)
            },

            // 1 : BR
            new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1)
            },

            // 2 : TL
            new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, -1),
                new Vector2Int(-1, 0),
                new Vector2Int(0, -1)
            },

            // 3 : TR
            new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1)
            }
        };

        Vector2Int[][] result = new Vector2Int[4][];

        // on parcours les offsets et regarde si ils sont valides avec notre index de base pour les ajouter aux resultats
        for (int i = 0; i < 4; i++)
        {
            result[i] = new Vector2Int[4];

            for (int j = 0; j < 4; j++)
            {
                if (_gridManager.CheckIndexValidation(regionMatriceIndex + offsets[i][j]))
                    result[i][j] = regionMatriceIndex + offsets[i][j];
            }
        }

        return result;
    }

}
