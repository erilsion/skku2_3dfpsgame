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
    public float AttackSpeed = 3f;
    public float AttackTimer = 0f;

    public float DetectDistance = 12f;
    public float ComebackDistance = 16f;
    public float ComebackPosition = 0.1f;
    public float AttackDistance = 3f;

    public float KnockbackForce = 1f;
    public float KnockbackDuration = 0.4f;
    public float HitDuration = 1.2f;
    public float DeathDuration = 2f;
    private Vector3 _knockbackDirection;

    private Vector3 _jumpStartPosition;
    private Vector3 _jumpEndPosition;
    private Coroutine _jumpCoroutine;
    [SerializeField] private float _jumpDuration = 0.9f;
    [SerializeField] private float _jumpHeight = 4f;

    private float _rageRate = 1.5f;
    private float _rageDoubleRate = 2f;
    private bool _isRaged = false;

    private static readonly int HashSpeed = Animator.StringToHash("Speed");


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

            _animator.SetTrigger("TraceToJump");

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
        if (State == EEliteMonsterState.Hit || State == EEliteMonsterState.Death)
        {
            return false;
        }

        Health.Decrease(damage.Value);
        _knockbackDirection = (transform.position - _player.transform.position).normalized;

        if (Health.Value > 0f)
        {
            State = EEliteMonsterState.Hit;
            Debug.Log("상태 전환: 어떤 상태 -> Hit");
            StartCoroutine(Hit_Coroutine());
        }
        else if (Health.Value <= Health.MaxValue && _isRaged == false)
        {
            State = EEliteMonsterState.Rage;
            Debug.Log("상태 전환: 어떤 상태 -> Death");
            StartCoroutine(Rage_Coroutine());
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
        StopCoroutine(Hit_Coroutine());
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
        Destroy(gameObject, DeathDuration);
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

            Vector3 pos = Vector3.Lerp(start, end, clampedT);

            float height = Mathf.Sin(clampedT * Mathf.PI) * _jumpHeight;
            pos.y += height;

            transform.position = pos;

            yield return null;
        }

        transform.position = end;
        _agent.CompleteOffMeshLink();

        _agent.Warp(end);

        _agent.updatePosition = true;
        _agent.isStopped = false;
        _jumpCoroutine = null;

        Debug.Log("상태 전환: Jump → Trace");
        State = EEliteMonsterState.Trace;
    }

    private IEnumerator Rage_Coroutine()
    {
        _isRaged = true;
        Damage = Damage * _rageRate;
        MoveSpeed = MoveSpeed * _rageDoubleRate;
        DetectDistance = DetectDistance * _rageDoubleRate;
        ComebackDistance = ComebackDistance * _rageDoubleRate;

        State = EEliteMonsterState.Trace;

        yield return null;
    }
}
