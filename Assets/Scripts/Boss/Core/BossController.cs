using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [SerializeField] private List<BossEffect> _bossEffects = new();
    [SerializeField] private float _minTimerBetweenEffects = 30f;
    [SerializeField] private float _maxTimerBetweenEffects = 120f;

    [SerializeField] private Sprite _windIcon;
    [SerializeField] private Sprite _earthIcon;
    [SerializeField] private Sprite _fireIcon;
    [SerializeField] private Sprite _waterIcon;

    [SerializeField] private Image _weaknessIcon;
    [SerializeField] private Image _resistanceIcon;

    Dictionary<RegionType, Sprite> _elemIcons = new Dictionary<RegionType, Sprite>();

    private float _randomTimer;
    private float _chrono = 0;


    private RegionType _weakness;
    private RegionType _resistance;

    public RegionType Resistance => _resistance;
    public RegionType Weakness => _weakness;

    private void Awake()
    {
        _elemIcons[RegionType.Water] = _waterIcon;
        _elemIcons[RegionType.Fire] = _fireIcon;
        _elemIcons[RegionType.Wind] = _windIcon;
        _elemIcons[RegionType.Rock] = _earthIcon;
    }

    private void Start()
    {
        SetAfinity();
        SetRandomTimer();
    }

    private void SetAfinity()
    {
        _weakness = _elemIcons.Keys.ElementAt(UnityEngine.Random.Range(0, _elemIcons.Count));
        
        if (_weakness == RegionType.None)
            _weakness = RegionType.Fire;

        switch (_weakness)
        {
            case RegionType.Fire:
                _resistance = RegionType.Rock;
                break;
            case RegionType.Wind:
                _resistance = RegionType.Water;
                break;
            case RegionType.Water:
                _resistance = RegionType.Fire;
                break;
            case RegionType.Rock:
                _resistance = RegionType.Wind;
                break;
            default:
                break;
                
        }

        _weaknessIcon.sprite = _elemIcons[_weakness];
        _resistanceIcon.sprite = _elemIcons[_resistance];
    }

    private void SetRandomTimer()
    {
        _randomTimer = UnityEngine.Random.Range(_minTimerBetweenEffects, _maxTimerBetweenEffects);
    }

    private void Update()
    {
        _chrono += Time.deltaTime;

        if ( _chrono >= _randomTimer) 
        {
            _chrono = 0;
            ApplyRandomEffect();
            SetRandomTimer();
        }
    }

    private void ApplyRandomEffect()
    {
        if ( _bossEffects.Count <= 0 ) return;

        // 1) on choisi un effet random dans la liste
        int randomIndex = UnityEngine.Random.Range(0, _bossEffects.Count);

        _bossEffects[randomIndex].Activate();


        // on active le visuel d'attaque

    }
}
