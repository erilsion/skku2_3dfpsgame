using UnityEngine;

public class Gold : MonoBehaviour
{
    public int Value = 1;

    [Header("곡선 옵션")]
    [SerializeField] private float _flyDuration = 0.35f;
    [SerializeField] private float _arcHeight = 1.2f;
    [SerializeField] private float _sideOffset = 0.5f;
    [SerializeField] private float _collectDistance = 0.25f;

    private float _time;
    private Vector3 _startPosition;
    private Vector3 _controlPosition;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Transform _target;
    private bool _isAttracting;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        _isAttracting = false;
        _target = null;
        _time = 0f;

        if (_rigidbody != null)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.detectCollisions = true;
        }

        if (_collider != null)
        {
            _collider.enabled = true;
        }
    }

    public void StartAttract(Transform player)
    {
        if (_isAttracting) return;
        if (_rigidbody == null) return;
        if (player == null) return;

        _isAttracting = true;
        _target = player;

        if (_collider != null)
        {
            _collider.enabled = false;
        }
        _rigidbody.detectCollisions = false;

        _rigidbody.isKinematic = true;

        CurveSetting();
    }

    private void Update()
    {
        if (!_isAttracting || _target == null) return;

        MovingCurve();
    }

    private void CurveSetting()
    {
        _time = 0f;
        _startPosition = transform.position;

        Vector3 endPosition = _target.position;
        Vector3 middle = (_startPosition + endPosition) * 0.5f;

        Vector3 toTarget = (endPosition - _startPosition);
        Vector3 side = Vector3.Cross(Vector3.up, toTarget.normalized);
        float sideRand = Random.Range(-_sideOffset, _sideOffset);

        _controlPosition = middle + Vector3.up * _arcHeight + side * sideRand;
    }

    private void MovingCurve()
    {
        Vector3 endPosition = _target.position;

        _time += Time.deltaTime / Mathf.Max(0.01f, _flyDuration);
        float timeTwo = Mathf.Clamp01(_time);

        Vector3 position = Bezier(_startPosition, _controlPosition, endPosition, timeTwo);
        transform.position = position;

        if (Vector3.Distance(transform.position, endPosition) <= _collectDistance || timeTwo >= 1f)
        {
            Collect();
        }
    }

    private Vector3 Bezier(Vector3 start, Vector3 curve, Vector3 end, float time)
    {
        float u = 1f - time;
        return (u * u) * start + (2f * u * time) * curve + (time * time) * end;
    }

    private void Collect()
    {
        if (_target == null) return;

        PlayerGold gold = _target.GetComponent<PlayerGold>();
        if (gold != null)
        {
            gold.AddGold(Value);
        }

        _isAttracting = false;
        _target = null;

        GoldPool.Instance.ReturnToPool(gameObject);
    }
}
