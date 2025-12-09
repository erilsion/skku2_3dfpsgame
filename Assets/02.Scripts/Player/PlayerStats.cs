using UnityEngine;
using UnityEngine.UI;
using Vitals;

public class PlayerStats : MonoBehaviour
{
    private Health _health;
    private Stamina _stamina;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    private float _staminaDecreasePerSecond = 20f;
    private float _staminaRecoverPerSecond = 10f;

    private PlayerMove _playerMove;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _stamina = GetComponent<Stamina>();
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = _health.MaxValue;
            _healthSlider.value = _health.Value;
        }

        if (_staminaSlider != null)
        {
            _staminaSlider.maxValue = _stamina.MaxValue;
            _staminaSlider.value = _stamina.Value;
        }
    }

    private void Update()
    {
        SprintMovement();
        UpdateUI();
    }
    void SprintMovement()
    {
        bool canSprint = _stamina.Value > 0f;
        bool isSprintKeyPressed = UnityEngine.Input.GetKey(KeyCode.LeftShift);

        bool isSprinting = isSprintKeyPressed && canSprint;

        if (isSprinting)
        {
            _stamina.Decrease(_staminaDecreasePerSecond * Time.deltaTime);

            if (_playerMove != null)
                _playerMove.SetRunMode();
        }
        else
        {
            _stamina.Increase(_staminaRecoverPerSecond * Time.deltaTime);

            if (_playerMove != null)
                _playerMove.SetWalkMode();
        }
    }

    void UpdateUI()
    {

        if (_healthSlider != null)
            _healthSlider.value = _health.Value;

        if (_staminaSlider != null)
            _staminaSlider.value = _stamina.Value;
    }
}
