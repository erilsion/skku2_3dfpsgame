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
    [SerializeField] private LayerMask _damageLayerMask;
    private float _destroyTime = 5f;

    [Header("날아가는 힘")]
    [SerializeField] private float _explosionPower = 1000f;
    [SerializeField] private float _explosionTorque = 90f;


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

        _rigidbody.AddForce(Vector3.up * _explosionPower);
        _rigidbody.AddTorque(Random.insideUnitSphere * _explosionTorque);

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius, _damageLayerMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage);
            }
        }

        CameraRecoil.Instance.DoRecoil();

        Destroy(gameObject, _destroyTime);
    }
}
