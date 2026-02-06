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


    private void OnDisable()
    {

        // Si c'était l'enfant 0 qui se désactive
        if (transform.GetSiblingIndex() == 0)
        {
            Transform parent = _dominoInfos.transform;
            Transform pivotChild = parent.GetChild(1); // l'autre enfant

            // Décalage entre le parent et le pivot 
            Vector3 offset = pivotChild.position - parent.position;

            //on deplace le parent vers le pivot
            parent.position += offset;

            // on deplace tt les enfants dans l'autre sens
            foreach (Transform child in parent)
            {
                child.position -= offset;
            }
        }
    }




}
