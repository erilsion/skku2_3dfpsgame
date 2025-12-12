using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drum : MonoBehaviour, IDamageable
{
    [Header("리지드바디")]
    private Rigidbody _rigidbody;

    [Header("체력")]
    [SerializeField] private ConsumableStat _health;

    [Header("공격력")]
    [SerializeField] private int _damage = 100;

    [Header("폭발 프리팹")]
    public GameObject ExplosionEffectPrefab;

    [Header("폭발 옵션")]
    private bool _hasExploded = false;
    [SerializeField] private float _explosionRadius = 10f;

    [Header("날아가는 힘")]
    [SerializeField] private float _explosionPower = 20f;


    private void Start()
    {
        _health.Initialize();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public bool TryTakeDamage(float Damage)
    {
        if (_health.Value <= 0) return false;
        if (_hasExploded) return false;

        _health.Decrease(Damage);

        if (_health.Value <= 0)
        {
            Explode();
        }
        return true;
    }

    private void Explode()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        GameObject effectObject = Instantiate(ExplosionEffectPrefab);
        effectObject.transform.position = transform.position;

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage);
            }
        }

        if (_rigidbody != null)
        {
            _rigidbody.AddForce(transform.up * _explosionPower, ForceMode.Impulse);
        }

        CameraRecoil.Instance.DoRecoil();
    }
}
