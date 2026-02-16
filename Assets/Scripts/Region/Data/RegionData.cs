using UnityEngine;

//[CreateAssetMenu(menuName = "RegionDataBase")]
//public class RegionDatabase : ScriptableObject
//{
//    public List<RegionData> AllRegionsData = new List<RegionData>();
//}

[CreateAssetMenu(menuName = "RegionDataBase")]
[System.Serializable]
public class RegionData : ScriptableObject
{
    public int RegionID;

    public RegionType Type;

    public Sprite RegionTerrain;

}

public enum RegionType
{
    None,
    Water,
    Rock,
    Fire,
    Wind
}