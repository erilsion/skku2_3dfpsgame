using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EliteMonster : PlayStateListener, IDamageable
{
    [Header("기본 상태")]
    public EEliteMonsterState State = EEliteMonsterState.Idle;

    [Header("NavMesh 에이전트")]
    [SerializeField] private NavMeshAgent _agent;

    [Header("플레이어 관련 옵션")]
    [SerializeField] private GameObject _player;
    private Transform _playerTransform;
    private PlayerStats _playerStats;
    private Animator _animator;

    [Header("능력치")]
    public float Damage = 30f;
    public ConsumableStat Health;

    public float MoveSpeed = 4f;
    public float AttackSpeed = 2.6f;
    public float AttackTimer = 0f;

    public float DetectDistance = 20f;
    public float AttackDistance = 6f;

    public float KnockbackForce = 1f;
    public float KnockbackDuration = 0.4f;
    public float HitDuration = 1.2f;
    public float DeathDuration = 2f;
    private Vector3 _knockbackDirection;

    [Header("점프 관련")]
    private Vector3 _jumpStartPosition;
    private Vector3 _jumpEndPosition;
    private Coroutine _jumpCoroutine;
    [SerializeField] private float _jumpDuration = 0.9f;
    [SerializeField] private float _jumpHeight = 4f;

    private int _goldAmount = 60;
    private float _dropForce = 0.02f;

    [Header("분노 관련")]
    [SerializeField] private float _rageRate = 1.5f;
    [SerializeField] private bool _isRaged = false;
    [SerializeField] private float _rageTime = 2.2f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _rageColor = Color.red;
    [SerializeField] private float _rageColorDuration = 0.6f;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _mpb;
    private Coroutine _rageColorCoroutine;

    private float _doubleRate = 2f;
    private float _halfRate = 0.5f;

    private static readonly int HashSpeed = Animator.StringToHash("Speed");

    [SerializeField] private GameObject _bloodEffectPrefab;


    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _agent.speed = MoveSpeed;
        _agent.stoppingDistance = AttackDistance;

        if (_player != null)
        {
            _playerStats = _player.GetComponent<PlayerStats>();
            _playerTransform = _player.transform;
        }

        _renderers = GetComponentsInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();

        _isRaged = false;
    }

    private void Update()
    {
        if (!IsPlaying) return;

        switch (State)
        {
            case EEliteMonsterState.Idle:
                Idle();
                break;

            case EEliteMonsterState.Trace:
                Trace();
                break;

            case EEliteMonsterState.Attack:
                Attack();
                break;

            case EEliteMonsterState.Jump:
                Jump();
                break;

            case EEliteMonsterState.Rage:
                Rage();
                break;
        }
    }

    private void LateUpdate()
    {
        if (!IsPlaying) return;
        UpdateMoveBlend();
    }

    private void UpdateMoveBlend()
    {
        if (_animator == null || _agent == null) return;

        float currentSpeed = _agent.velocity.magnitude;

        float normalized = (MoveSpeed <= 0f) ? 0f : Mathf.Clamp01(currentSpeed / MoveSpeed);

        if (State == EEliteMonsterState.Attack || State == EEliteMonsterState.Hit || State == EEliteMonsterState.Death) normalized = 0f;

        _animator.SetFloat(HashSpeed, normalized, 0.1f, Time.deltaTime);
    }

    private void Idle()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EEliteMonsterState.Trace;
            Debug.Log("상태 전환: Idle -> Trace");
        }
    }

    private void Trace()
    {
        if (_player == null)
        {
            State = EEliteMonsterState.Idle;
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        _agent.SetDestination(_playerTransform.position);

        if (distance <= AttackDistance)
        {
            State = EEliteMonsterState.Attack;
        }

        if (_agent.isOnOffMeshLink)
        {
            OffMeshLinkData linkData = _agent.currentOffMeshLinkData;
            _jumpStartPosition = linkData.startPos;
            _jumpEndPosition = linkData.endPos;

            _animator.SetTrigger("Jump");

            if (NavMesh.SamplePosition(_jumpEndPosition, out var hit, 1.0f, NavMesh.AllAreas))
            {
                _jumpEndPosition = hit.position;
            }

            State = EEliteMonsterState.Jump;
            return;
        }
    }

    private void Attack()
    {
        if (_player == null)
        {
            State = EEliteMonsterState.Idle;
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > AttackDistance)
        {
            State = EEliteMonsterState.Trace;
            return;
        }

        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackSpeed)
        {
            _animator.SetTrigger("Attack");

            if (_playerStats != null)
            {
                Damage damage = new Damage()
                {
                    Value = Damage,
                    HitPoint = transform.position
                };

                _playerStats.TryTakeDamage(Damage);
                Debug.Log("플레이어 공격!");
            }

            AttackTimer = 0f;
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        float nextHp = Health.Value - damage.Value;

        if (!_isRaged && nextHp <= Health.MaxValue * _halfRate)
        {
            Health.Decrease(damage.Value);
            _knockbackDirection = (transform.position - _player.transform.position).normalized;
            EnterRage();
            return true;
        }

        if (State == EEliteMonsterState.Hit || State == EEliteMonsterState.Death || State == EEliteMonsterState.Rage)
        {
            return false;
        }

        GameObject bloodEffect = Instantiate(_bloodEffectPrefab, damage.HitPoint, transform.rotation, transform);
        bloodEffect.transform.forward = damage.Normal;

        Health.Decrease(damage.Value);
        _knockbackDirection = (transform.position - _player.transform.position).normalized;

        if (Health.Value > 0f)
        {
            State = EEliteMonsterState.Hit;
            Debug.Log("상태 전환: 어떤 상태 -> Hit");
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            State = EEliteMonsterState.Death;
            Debug.Log("상태 전환: 어떤 상태 -> Death");
            StartCoroutine(Death_Coroutine());
        }

        return true;
    }

    private IEnumerator Hit_Coroutine()
    {
        _animator.SetTrigger("Hit");

        _agent.isStopped = true;
        _agent.ResetPath();

        float timer = 0f;
        while (timer < KnockbackDuration)
        {
            _agent.Move(_knockbackDirection * KnockbackForce * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(HitDuration);

        _agent.isStopped = false;

        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EEliteMonsterState.Trace;
        }
        else
        {
            State = EEliteMonsterState.Idle;
        }
    }

    private IEnumerator Death_Coroutine()
    {
        _animator.SetTrigger("Death");

        _agent.isStopped = true;

        float timer = 0f;
        while (timer < KnockbackDuration)
        {
            _agent.Move(_knockbackDirection * KnockbackForce * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        DropGolds();
        Destroy(gameObject, DeathDuration);
    }

    public void DropGolds()
    {
        for (int i = 0; i < _goldAmount; i++)
        {
            GameObject gold = GoldPool.Instance.GetFromPool(transform.position, Quaternion.identity);

            Rigidbody rigidbody = gold.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;

                rigidbody.AddForce(direction * _dropForce, ForceMode.Impulse);
            }
        }
    }

    private void Jump()
    {
        if (_jumpCoroutine != null) return;

        _agent.isStopped = true;
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        _jumpCoroutine = StartCoroutine(Jump_Coroutine(_jumpStartPosition, _jumpEndPosition));
    }

    private IEnumerator Jump_Coroutine(Vector3 start, Vector3 end)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / _jumpDuration;
            float clampedT = Mathf.Clamp01(t);

            Vector3 position = Vector3.Lerp(start, end, clampedT);

            float height = Mathf.Sin(clampedT * Mathf.PI) * _jumpHeight;
            position.y += height;

            transform.position = position;

            yield return null;
        }

        transform.position = end;
        _agent.CompleteOffMeshLink();

        _agent.Warp(end);

        _agent.updatePosition = true;
        _agent.updateRotation = true;
        _agent.isStopped = false;
        _jumpCoroutine = null;

        Debug.Log("상태 전환: Jump → Trace");
        State = EEliteMonsterState.Trace;
    }

    private void EnterRage()
    {
        _isRaged = true;
        State = EEliteMonsterState.Rage;

        _agent.isStopped = true;
        _agent.ResetPath();

        if (_rageColorCoroutine != null)
        {
            StopCoroutine(_rageColorCoroutine);
        }

        _rageColorCoroutine = StartCoroutine(
            LerpColor_Coroutine(_normalColor, _rageColor, _rageColorDuration)
        );

        StartCoroutine(Rage_Coroutine());
    }

    private void Rage()
    {
        // Rage 연출 중: 이동/공격 불가
    }

    private IEnumerator Rage_Coroutine()
    {
        _animator.SetTrigger("Rage");

        Damage *= _rageRate;
        MoveSpeed *= _doubleRate;
        _agent.speed = MoveSpeed;
        AttackSpeed *= _halfRate;
        DetectDistance *= _rageRate;
        HitDuration *= _halfRate;

        yield return new WaitForSeconds(_rageTime);

        _agent.isStopped = false;
        State = EEliteMonsterState.Trace;
    }

    private IEnumerator LerpColor_Coroutine(Color from, Color to, float duration)
    {
        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / duration;
            ApplyColor(Color.Lerp(from, to, time));
            yield return null;
        }

        ApplyColor(to);
    }

    private void ApplyColor(Color color)
    {
        foreach (var renderer in _renderers)
        {
            renderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_BaseColor", color);
            renderer.SetPropertyBlock(_mpb);
        }
    }
}
