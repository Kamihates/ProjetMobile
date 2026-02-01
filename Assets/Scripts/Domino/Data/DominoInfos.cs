using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DominoInfos 
{
    private List<RegionData> regions = new();
    public List<RegionData> Regions { get => regions; set => regions = value; }
}
