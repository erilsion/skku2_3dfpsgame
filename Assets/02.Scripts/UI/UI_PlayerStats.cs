using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_PlayerStats : MonoBehaviour
{
    [Header("플레이어 스탯")]
    [SerializeField] private PlayerStats _stats;

    [Header("체력바")]
    [SerializeField] private Slider _healthFrontSlider;
    [SerializeField] private Slider _healthBackSlider;
    [SerializeField] private Image _healthFrontSliderFill;

    [Header("스태미너바")]
    [SerializeField] private Slider _staminaSlider;

    [Header("체력바 딜레이 옵션")]
    [SerializeField] private float _backDelay = 0.2f;
    [SerializeField] private float _backLerpSpeed = 1.6f;
    [SerializeField] private float _colorDelay = 0.14f;

    [Header("히트 스크린")]
    [SerializeField] private Image _hitScreenImage;
    private float _hitScreenTime = 0.2f;

    private Coroutine _backRoutine;
    private Coroutine _hitScreenRoutine;
    private float _lastHealth01;


    private void Awake()
    {
        _hitScreenImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        _healthFrontSliderFill.color = Color.red;

        float h01 = GetHealth01();
        _lastHealth01 = h01;

        if (_healthFrontSlider != null) _healthFrontSlider.value = h01;
        if (_healthBackSlider != null) _healthBackSlider.value = h01;
    }

    void Update()
    {
        if (GameManager.Instance.State == EGameState.GameOver || _stats == null)
        {
            StopHitScreen();
            StopBackRoutine();
            SetHealth(0f);
            return;
        }

        float target = GetHealth01();

        if (target < _lastHealth01)
        {
            TryPlayHitScreen();
        }
        _lastHealth01 = target;

        if (_healthFrontSlider != null) _healthFrontSlider.value = target;

        if (_healthBackSlider == null) return;

        if (target >= _healthBackSlider.value)
        {
            StopBackRoutine();
            _healthBackSlider.value = target;
        }
        else
        {
            if (_backRoutine == null)
            {
                _backRoutine = StartCoroutine(BackBarFollow_Coroutine());
            }
            StopCoroutine(HitScreen_Coroutine());
        }

        if (_staminaSlider != null)
        {
            _staminaSlider.value = GaugeChanged(_stats.Stamina.Value, _stats.Stamina.MaxValue);
        }
    }

    private void TryPlayHitScreen()
    {
        if (_hitScreenRoutine != null) return;
        _hitScreenRoutine = StartCoroutine(HitScreen_Coroutine());
    }

    private void StopHitScreen()
    {
        if (_hitScreenRoutine != null)
        {
            StopCoroutine(_hitScreenRoutine);
            _hitScreenRoutine = null;
        }
    }

    private IEnumerator HitScreen_Coroutine()
    {
        _hitScreenImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(_hitScreenTime);
        _hitScreenImage.gameObject.SetActive(false);

        _hitScreenRoutine = null;
    }

    private IEnumerator BackBarFollow_Coroutine()
    {
        _healthFrontSliderFill.color = Color.white;

        yield return new WaitForSeconds(_colorDelay);

        _healthFrontSliderFill.color = Color.red;

        yield return new WaitForSeconds(_backDelay);

        while (true)
        {
            if (GameManager.Instance.State == EGameState.GameOver || _stats == null)
                yield break;

            float target = GetHealth01();

            if (target >= _healthBackSlider.value)
                break;

            _healthBackSlider.value = Mathf.MoveTowards(_healthBackSlider.value, target, _backLerpSpeed * Time.deltaTime);
            yield return null;
        }

        _backRoutine = null;
    }

    private void StopBackRoutine()
    {
        if (_backRoutine != null)
        {
            StopCoroutine(_backRoutine);
            _backRoutine = null;
        }
    }

    private void SetHealth(float value01)
    {
        if (_healthFrontSlider != null) _healthFrontSlider.value = value01;
        if (_healthBackSlider != null) _healthBackSlider.value = value01;
    }

    private float GetHealth01()
    {
        return GaugeChanged(_stats.Health.Value, _stats.Health.MaxValue);
    }

    private float GaugeChanged(float value, float max)
    {
        if (max <= 0f) return 0f;
        return Mathf.Clamp01(value / max);
    }
}
