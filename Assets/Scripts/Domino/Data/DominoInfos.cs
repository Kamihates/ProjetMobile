using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class DominoInfos 
{
    [SerializeField, NaughtyAttributes.ReadOnly] private List<RegionData> regions = new();
    public List<RegionData> Regions { get => regions; set => regions = value; }

    

}
