using UnityEngine;

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

        // FindObjectsOfType을 쓰게 된다면...
        // 1. 씬을 모두 순회하면서 게임 오브젝트를 찾는다. -> 몬스터가 1000마리면 1000번 순회
        // 2. 모든 몬스터를 순회하면서 거리를 측정한다. -> 500마리면 500번 순회

        // OverlapSphere: 가상의 구를 만들어서, 그 구 영역 안에 있는 모든 콜라이더를 찾아서 배열로 반환한다.
        Collider[] hits = Physics.OverlapSphere(transform.position, _explosionRadius, _damageLayerMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage);
            }
        }

        CameraRecoil.Instance.DoRecoil();

        BombManager.Instance.RemoveBomb();
        BombPool.Instance.ReturnToPool(gameObject);
    }
}
