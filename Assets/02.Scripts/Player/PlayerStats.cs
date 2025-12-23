using UnityEngine;

// 플레이어의 '스탯'을 관리하는 컴포넌트
public class PlayerStats : MonoBehaviour
{
    [Header("스태미나")]
    public ConsumableStat Stamina;

    [Header("체력")]
    public ConsumableStat Health;

    [Header("스탯")]
    public ValueStat Damage;
    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;
    public ValueStat JumpPower;

    private Animator _animator;

    public event System.Action<float, float> OnHealthChanged;

    private void Start()
    {
        Health.Initialize();
        Stamina.Initialize();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        float deltaTime = Time.deltaTime;
        HealthRegenerate(deltaTime);
        Stamina.Regenerate(deltaTime);
    }

    // 체력 닳아있으면 자동 회복
    private void HealthRegenerate(float time)
    {
        if (Health.Value >= Health.MaxValue) return;
        Health.Regenerate(time);
        UpdateHealth();
    }

    public bool TryTakeDamage(float damage)
    {
        bool depletedNow = Health.ApplyDamage(damage);

        UpdateHealth();

        if (depletedNow)
        {
            _animator.SetTrigger("Death");
            GameManager.Instance.GameOver();
        }

        return true;
    }

    private void UpdateHealth()
    {
        OnHealthChanged?.Invoke(Health.Value, Health.MaxValue);
    }
}
