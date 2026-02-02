using NUnit.Framework;
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
                    // on récupère
                }
            }
        }
    }

    //private List<Vector2Int> GetComboNeighbors(RegionPiece region, int squarePart)
    //{
    //    switch (squarePart)
    //    {
    //        case 0:
    //            // carré de 4 BL
    //            Vector2 


    //            break;
    //        case 1:
    //            break;
    //        case 2:
    //            break;
    //        case 3:
    //            break;
    //        default:
    //            return null;
    //    }
    //}
}
