using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class DominoInfos 
{
    [SerializeField, NaughtyAttributes.ReadOnly] private List<RegionData> regions = new();
    public List<RegionData> Regions { get => regions; set => regions = value; }

    private bool _isDominoFusion = false;
    public bool IsDominoFusion { get => _isDominoFusion; set => _isDominoFusion = value; }

    private int _fusionPower;
    // fusion power = force du t1 (4/6/8/9)
    public int FusionPower { get => _fusionPower; set => _fusionPower = value; }

    /// <summary>
    /// Calcule son multiplicateur de multiplicateur
    /// </summary>
    /// <param name="regions"></param>
    public void SetPower(int regions)
    {
        switch (regions)
        {
            case 4:
                _fusionPower = 1;
                break;
            case 6:
                _fusionPower = 2;
                break;
            case 8:
                _fusionPower = 3;
                break;
            case 9:
                _fusionPower = 4;
                break;
            default:
                _fusionPower = 6;
                break;
        }
    }
}



