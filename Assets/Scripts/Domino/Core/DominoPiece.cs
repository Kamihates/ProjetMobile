using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DominoPiece : MonoBehaviour
{
    private int _pieceUniqueId;
    public int PieceUniqueId { get => _pieceUniqueId; }


    private int _currentRotation = 0;

    List<RegionData> _dominoRegion;

    public void Rotate()
    {
        switch (_currentRotation) 
        {
            case 0:
                // on trourne vers la droite
                _currentRotation = 1;
                break;
            case 1:
                // on trourne vers le bas
                _currentRotation = 2;
                break;
            case 2:
                // on trourne vers la gauche
                _currentRotation = 3;
                break;
            case 3:
                // on trourne vers le haut
                _currentRotation = 0;
                break;
            default:
                break;

        }
    }

    public void Init(int _UniqueId, int rotation, List<RegionData> dominoRegions)
    {
        _pieceUniqueId = _UniqueId;
        _currentRotation = rotation;
        _dominoRegion = dominoRegions;

        UpdateVisual();
    }

    private void UpdateVisual()
    {   
        // région 1 
        for (int r = 0; r < _dominoRegion.Count; r++)
        {
            if (transform.GetChild(r).TryGetComponent<RegionPiece>(out RegionPiece region))
            {
                region.Init(_dominoRegion[r]);

                region.gameObject.SetActive(true);
            }
    
        }
        
    }


}
