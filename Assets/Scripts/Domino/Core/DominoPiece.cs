using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DominoPiece : MonoBehaviour
{
    [SerializeField] private DominoPieceVisual visualController;

    private int _pieceUniqueId;
    private int _currentRotation = 0;
    private DominoInfos _data;

    public DominoPieceVisual Visual => visualController;
    public int PieceUniqueId { get => _pieceUniqueId; }
    public int Rotation { get => _currentRotation; set => _currentRotation = value; }
    public DominoInfos Data => _data;

    public void Init(int _UniqueId, int rotation, DominoInfos dominoRegions)
    {
        _pieceUniqueId = _UniqueId;
        _data = dominoRegions;
        _currentRotation = rotation;

        visualController.Init(this);
        visualController.UpdateVisual();
        visualController.Rotate();
    }






}
