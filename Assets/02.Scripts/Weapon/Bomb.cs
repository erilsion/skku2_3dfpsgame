using UnityEditor.PackageManager;
using UnityEngine;
using static PlayerGunFire;

public class Bomb : MonoBehaviour
{
    [Header("폭발 프리팹")]
    public GameObject ExplosionEffectPrefab;

    [Header("리지드바디")]
    private Rigidbody _rigidbody;

    [Header("폭발 옵션")]
    private bool _hasExploded = false;
    [SerializeField] private float _explosionRadius = 4f;
    [SerializeField] private LayerMask _damageLayerMask;

    [Header("데미지")]
    [SerializeField] private int _damage = 40;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _hasExploded = false;
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExploded) return;
        _hasExploded = true;

        GameObject effectObject = Instantiate(ExplosionEffectPrefab);
        effectObject.transform.position = transform.position;

        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius, _damageLayerMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IBombDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage);
            }
        }

        CameraRecoil.Instance.DoRecoil();

        BombManager.Instance.RemoveBomb();
        BombPool.Instance.ReturnToPool(gameObject);
    }

    public interface IBombDamageable
    {
        bool TryTakeDamage(float damage);
    }
}
