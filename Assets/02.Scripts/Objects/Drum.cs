using System;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drum : MonoBehaviour, IDamageable
{
    private Rigidbody _rigidbody;

    [Header("체력")]
    [SerializeField] private ConsumableStat _health;

    [Header("공격력")]
    [SerializeField] private float _damage = 100;

    [Header("폭발 프리팹")]
    [SerializeField] private GameObject _explosionEffectPrefab;

    [Header("폭발 옵션")]
    private bool _hasExploded = false;
    [SerializeField] private float _explosionRadius = 10f;
    [SerializeField] private LayerMask _damageLayerMask;
    [SerializeField] private float _destroyTime = 5f;
    public event Action<Drum> OnExploded;

    [Header("날아가는 힘")]
    [SerializeField] private float _explosionPower = 1000f;
    [SerializeField] private float _explosionTorque = 90f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _health.Initialize();
    }


    private void Explode()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        OnExploded?.Invoke(this);

        Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);

        _rigidbody.AddForce(Vector3.up * _explosionPower);
        _rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * _explosionTorque);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius, _damageLayerMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                Damage damage = new Damage()
                {
                    Value = _damage,
                    HitPoint = transform.position
                };
                damageable.TryTakeDamage(damage);
            }
        }
        Destroy(gameObject, _destroyTime);
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (_hasExploded) return false;


        bool depletedNow = _health.ApplyDamage(damage.Value);

        if (depletedNow)
        {
            Explode();
        }

        return true;
    }
}
