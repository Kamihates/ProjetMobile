using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient healthGradient;

    [SerializeField] private MonsterHealth monsterHealth;

    private void Start()
    {
        monsterHealth.OnHealthUpdate += UpdateHealthBar;
        UpdateHealthBar(monsterHealth.CurrentHealth, monsterHealth.MaxHealth);
    }

    private void OnDestroy()
    {
        monsterHealth.OnHealthUpdate -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int current, int max)
    {
        float normalize = (float)current / max;
        healthSlider.value = normalize;
        fillImage.color = healthGradient.Evaluate(normalize);
    }
}
