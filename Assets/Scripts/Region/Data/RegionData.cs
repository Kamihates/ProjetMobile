using NUnit.Framework;
using System.Collections.Generic;
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
    public Sprite RegionOverlay;

}

public enum RegionType
{
    None,
    Foret,
    Ocean
}