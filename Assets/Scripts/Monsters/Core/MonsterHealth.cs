using UnityEngine;
using NaughtyAttributes;
using System;
using GooglePlayGames;

public class MonsterHealth : MonoBehaviour
{
    [Header("Health"), SerializeField] private float maxHealth = 100;
    [SerializeField, Foldout("Debug"), ReadOnly] private float currentHealth;

    [SerializeField] private DominoCombos dominoCombos;

    private float _chrono = 0;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public Action<float, float> OnHealthUpdate;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthUpdate?.Invoke(currentHealth, maxHealth);
    }

    private void Start()
    {
        if (dominoCombos != null)
        {
            dominoCombos.OnComboDamage += TakeDamage;

        }
    }

    private void Update()
    {
        _chrono += Time.deltaTime;
    }

    private void OnDestroy()
    {
        if (dominoCombos != null)
            dominoCombos.OnComboDamage -= TakeDamage;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthUpdate?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            if (_chrono <= 120)
            {
                _chrono = 0;
                // succes "Archmage"
                PlayGamesPlatform.Instance.ReportProgress("CgkIjP3qhoIaEAIQAg", 100.0f, (bool success) =>
                {
                    if (success)
                        Debug.Log("Succès débloqué !");
                    else
                        Debug.Log("Échec du déblocage du succès.");
                });
            }




            // succes "first victory"
            PlayGamesPlatform.Instance.ReportProgress("CgkIjP3qhoIaEAIQAQ", 100.0f, (bool success) =>
            {
                if (success)
                    Debug.Log("Succès débloqué !");
                else
                    Debug.Log("Échec du déblocage du succès.");
            });
            GameManager.Instance.GameWon();
        }
    }
}
