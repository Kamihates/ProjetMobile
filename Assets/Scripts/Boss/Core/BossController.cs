using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private List<BossEffect> _bossEffects = new();
    [SerializeField] private float _minTimerBetweenEffects = 30f;
    [SerializeField] private float _maxTimerBetweenEffects = 120f;

    private float _randomTimer;
    private float _chrono = 0;

    private void Start()
    {
        SetRandomTimer();
    }

    private void SetRandomTimer()
    {
        _randomTimer = Random.Range(_minTimerBetweenEffects, _maxTimerBetweenEffects);
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
        int randomIndex = Random.Range(0, _bossEffects.Count);

        _bossEffects[randomIndex].Activate();


        // on active le visuel d'attaque

    }
}
