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

    public Action<int, int> OnHealthUpdate;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthUpdate?.Invoke(currentHealth, maxHealth);
    }

    private void OnEnable()
    {
        if (dominoCombos != null)
            dominoCombos.OnComboDamage += TakeDamage;
    }

    private void OnDisable()
    {
        if (dominoCombos != null)
            dominoCombos.OnComboDamage -= TakeDamage;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthUpdate?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) 
            Debug.Log("Monster dead");
    }
}
