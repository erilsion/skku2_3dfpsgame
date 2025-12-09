using UnityEngine;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private SliderJoint2D _healthSlider;
    [SerializeField] private SliderJoint2D _staminaSlider;


    void Update()
    {
        //_healthSlider.value = _stats.Health.Value / _stats.Health.MaxValue;
        //_staminaSlider.value = _stats.Stamina.Value / _stats.Stamina.MaxValue;
    }
}
