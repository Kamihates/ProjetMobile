using UnityEngine;

public class RegionPiece : MonoBehaviour
{
    private int _dominoUniqueId;
    public int PieceUniqueId { get => _dominoUniqueId; }

    private bool _isFusionT1 = false;
    public bool IsT1 { get => _isFusionT1; }

    private RegionData _regionData;
    public RegionData Region {get => _regionData; set => _regionData = value; }

    private DominoPiece _dominoInfos;
    public DominoPiece DominoParent => _dominoInfos;

    [SerializeField] private SpriteRenderer _terrainRenderer;
    [SerializeField] private SpriteRenderer _overlayRenderer;

    public void Init(RegionData data, bool t1, DominoPiece domino = null)
    {
        _regionData = data;
        _isFusionT1 = t1;
        _dominoInfos = domino;

        _terrainRenderer.sprite = data.RegionTerrain;
        // _overlayRenderer.sprite = data.RegionOverlay;

        if (_terrainRenderer != null)
            _terrainRenderer.sortingOrder = 1;

        if (_overlayRenderer != null)
            _overlayRenderer.sortingOrder = 2;

        GeneralVisualController.Instance.FitSpriteInCell(_terrainRenderer);
        GeneralVisualController.Instance.FitSpriteInCell(_overlayRenderer);
    }

    



}
