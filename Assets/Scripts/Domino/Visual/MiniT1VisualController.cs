using UnityEngine;

public class MiniT1VisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer Region1Terrain;
    [SerializeField] private SpriteRenderer Region2Terrain;

    public void Init(RegionData region)
    {
        if (Region1Terrain == null && Region2Terrain == null) return;

        Region1Terrain.sprite = region.RegionTerrain;
        Region2Terrain.sprite = region.RegionTerrain;



    }
}
