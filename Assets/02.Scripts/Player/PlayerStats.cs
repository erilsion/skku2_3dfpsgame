using UnityEngine;

// 플레이어의 '스탯'을 관리하는 컴포넌트
public class PlayerStats : MonoBehaviour
{
    // 도메인: 특정 분야의 지식

    // 1. 옵저버 패턴은 어떻게 해야지?
    // 2. ConsumableStat의 Regenerate는 PlayerStats에서만 호출 가능하게 하고 싶다. 다른 속성/기능은 다른 클래스에서 사용할 수 있다.

    [Header("스태미나")]  // 소모 가능한 스탯
    public ConsumableStat Stamina;

    [Header("체력")]  // 소모 가능한 스탯
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
        Regenerated(deltaTime);
        Stamina.Regenerate(deltaTime);
    }

    private void Regenerated(float time)
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
