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
        if (GameManager.Instance.IsInfiniteState)
        {
            healthSlider.gameObject.SetActive(false);

            // on met le total damage a la place
        }
        else
        {
            healthSlider.gameObject.SetActive(true);
            monsterHealth.OnHealthUpdate += UpdateHealthBar;
            UpdateHealthBar(monsterHealth.CurrentHealth, monsterHealth.MaxHealth);
        }
  
    }

    private void OnDestroy()
    {
        monsterHealth.OnHealthUpdate -= UpdateHealthBar;
    }


    private void UpdateHealthBar(float current, float max)
    {
        float normalize = (float)current / max;
        healthSlider.value = normalize;
        fillImage.color = healthGradient.Evaluate(normalize);
    }
}
