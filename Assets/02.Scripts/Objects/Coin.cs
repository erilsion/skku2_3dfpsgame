using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("획득 옵션")]
    public int Value = 1;
    [SerializeField] private float _attractSpeed = 12f;
    
    private Rigidbody _rigidbody;
    private Transform _target;
    private bool _isAttracting;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _isAttracting = false;
        _target = null;

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = false;
        }
    }

    public void StartAttract(Transform player)
    {
        if (_isAttracting) return;
        if (_rigidbody == null) return;

        _isAttracting = true;
        _target = player;

        _rigidbody.isKinematic = true;
    }

    private void Update()
    {
        if (!_isAttracting || _target == null) return;

        transform.position = Vector3.MoveTowards(transform.position,_target.position,_attractSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isAttracting && other.CompareTag("Player"))
        {
            PlayerGold gold = other.GetComponent<PlayerGold>();
            if (gold != null)
            {
                gold.AddGold(Value);
            }

            CoinPool.Instance.ReturnToPool(gameObject);
        }
    }
}
