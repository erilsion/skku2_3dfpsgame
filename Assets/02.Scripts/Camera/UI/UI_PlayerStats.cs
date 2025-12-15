using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;


    void Update()
    {
        if (GameManager.Instance.State == EGameState.GameOver)
        {
            if (_healthSlider != null) _healthSlider.value = 0f;
            return;
        }

        if (_stats == null)
        {
            if (_healthSlider != null) _healthSlider.value = 0f;
            return;
        }

        if (_healthSlider != null)
        {
            _healthSlider.value = GaugeChanged(_stats.Health.Value, _stats.Health.MaxValue);
        }

        if (_staminaSlider != null)
        {
            _staminaSlider.value = GaugeChanged(_stats.Stamina.Value, _stats.Stamina.MaxValue);
        }
    }

    private float GaugeChanged(float value, float max)
    {
        if (max <= 0f) return 0f;
        return Mathf.Clamp01(value / max);
    }
}
