using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterVisualControlller : MonoBehaviour
{
    [SerializeField] private float _RedFadeDuration = 0.1f;
    [SerializeField] private float _RedTimeDisplay = 0.2f;

    [SerializeField] private SpriteRenderer _monsterRenderer;
    private Material _monsterRedShader;

    [SerializeField] private CanvasGroup _attackNameCG;
    [SerializeField] private Animator _monsterAnimator;

    [SerializeField] private TextMeshProUGUI _Score;
    private float _totalScore = 0;


    private void Awake()
    {
        if (_monsterRenderer != null)
        {
            _monsterRedShader = _monsterRenderer.sharedMaterial;
            _monsterRedShader.SetFloat("_Colorbright", 0);
        }
    }
    private void Start()
    {
        if (GameManager.Instance.IsInfiniteState)
        {
            _Score.text = _totalScore.ToString();
            _Score.gameObject.SetActive(true);
        }
        else
        {
            _Score.gameObject.SetActive(false);
        }
    }

    public void AddScore(float damage)
    {
        _totalScore += damage;
        _Score.text = _totalScore.ToString();
        StartCoroutine(ScoreEffect());
    }


    public IEnumerator ScoreEffect()
    {
        float _time = 0;
        float _duration = 0.3f;

        float currentSize = _Score.fontSize;

        while (_time < _duration)
        {
            _time += Time.deltaTime;
            float t = _time / _duration;

            _Score.fontSize = Mathf.Lerp(currentSize, currentSize + 50, Mathf.Sin(t)); 

            yield return null;
        }

        _Score.fontSize = currentSize;

    }

    public IEnumerator TakeVisualDamage()
    {
        // shader rouge
        StartCoroutine(TakeDmg(true));
        _monsterAnimator.SetTrigger("damaged");
        yield return new WaitForSeconds(_RedTimeDisplay);
        StartCoroutine(TakeDmg(false));
    }

    
    IEnumerator TakeDmg(bool fade)
    {
        float _time = 0;

        while (_time < _RedFadeDuration)
        {
            _time += Time.deltaTime;
            float t = _time / _RedFadeDuration;

            _monsterRedShader.SetFloat("_Colorbright", Mathf.Lerp(fade ? 0 : 1, fade ? 1 : 0, t));
            yield return null;
        }

        _monsterRedShader.SetFloat("_Colorbright", fade ? 1 : 0);
        
    }

    public void ShowAttack(string attackName)
    {
        if (_attackNameCG.TryGetComponent(out TextMeshProUGUI nametxt))
        {
            nametxt.text = attackName;
            StartCoroutine(UIAnimations.Instance.DisplayForXSeconds(2, 0.1f, _attackNameCG));
        }
    }

    
}
