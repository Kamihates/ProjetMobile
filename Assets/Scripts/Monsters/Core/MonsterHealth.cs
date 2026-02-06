using UnityEngine;
using NaughtyAttributes;
using System;

public class MonsterHealth : MonoBehaviour
{
    [Header("Health"), SerializeField] private int maxHealth = 100;
    [SerializeField, Foldout("Debug"), ReadOnly] private int currentHealth;

    [SerializeField] private DominoCombos dominoCombos;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

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
            Debug.Log("Monster dead");
    }
}
