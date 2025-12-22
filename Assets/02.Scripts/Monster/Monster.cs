using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class Monster : PlayStateListener, IDamageable
{
    // 유한 상태머신 설명
    #region Comment
    /*
    // 목표: 처음에는 가만히 있지만 플레이어가 다가가면 쫓아오는 좀비 몬스터를 만들고 싶다.
    //       ㄴ 쫓아오다가 너무 멀어지면 제자리로 돌아간다.

    // Idle: 가만히 있는다.
    // (플레이어가 가까이 오면) = 컨디션, 트랜지션
    // Trace: 플레이어를 쫓아간다.
    // (플레이어가 너무 멀어지면)
    // Comeback: 제자리로 돌아간다.  -> (제자리에 도착했다면) Idle
    // 그 외의 상태: 공격, 피격, 죽음...

    // 몬스터 인공지능(AI): 사람처럼 행동하는 똑똑한 시스템(알고리즘)
    // - 규칙 기반 인공지능: 정해진 규칙에 따라 조건문/반복문 등을 이용해서 코딩하는 것
    // -> FSM(유한 상태머신), BT(행동 트리)
    // - 학습 기반 인공지능: ML(머신러닝), DL(딥러닝), 강화학습

    // Finite State Machine(유한 상태머신)
    // 유한개의 상태를 가지고 있고, 상태마다 동작이 다르다.
    */
    #endregion

    [Header("기본 상태")]
    public EMonsterState State = EMonsterState.Idle;

    [Header("NavMesh 에이전트")]
    [SerializeField] private NavMeshAgent _agent;

    private Vector3 _spawnPosition;

    [Header("플레이어 관련 옵션")]
    [SerializeField] private GameObject _player;
    private Transform _playerTransform;
    private PlayerStats _playerStats;
    private Animator _animator;

    [Header("능력치")]
    public float Damage = 10f;
    public ConsumableStat Health;

    public float MoveSpeed = 5f;
    public float AttackSpeed = 2f;
    public float AttackTimer = 0f;

    public float DetectDistance = 12f;
    public float ComebackDistance = 16f;
    public float ComebackPosition = 0.1f;
    public float AttackDistance = 2.2f;

    public float KnockbackForce = 2f;
    public float KnockbackDuration = 0.4f;
    public float HitDuration = 1.2f;
    public float DeathDuration = 2f;
    private Vector3 _knockbackDirection;

    [Header("순찰 관련")]
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private float _patrolWaitTime = 3f;
    private int _currentPatrolIndex = 0;
    private float _patrolWaitTimer = 0f;
    private float _patrolArriveDistance = 0.1f;
    private float _idleToPatrolDelay = 2f;
    private float _idleTimer = 0f;

    private Vector3 _jumpStartPosition;
    private Vector3 _jumpEndPosition;

    private Coroutine _jumpCoroutine;
    [SerializeField] private float _jumpDuration = 0.9f;
    [SerializeField] private float _jumpHeight = 4f;

    [SerializeField] private GameObject _bloodEffectPrefab;

    private int _coinAmount = 20;
    private float _dropForce = 0.01f;

    private static readonly int HashSpeed = Animator.StringToHash("Speed");


    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }

        _agent.speed = MoveSpeed;
        _agent.stoppingDistance = AttackDistance;

        if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
        {
            _spawnPosition = hit.position;
        }
        else
        {
            _spawnPosition = transform.position;
        }

        if (_player != null)
        {
            _playerStats = _player.GetComponent<PlayerStats>();
            _playerTransform = _player.transform;
        }
    }


    private void Update()
    {
        if (!IsPlaying) return;

        // 몬스터의 상태에 따라 다른 행동을 한다. (다른 메서드를 호출한다.)
        switch (State)
        {
            case EMonsterState.Idle:
                Idle();
                break;

            case EMonsterState.Trace:
                Trace();
                break;

            case EMonsterState.Comeback:
                Comeback();
                break;

            case EMonsterState.Attack:
                Attack();
                break;

            case EMonsterState.Patrol:
                Patrol();
                break;

            case EMonsterState.Jump:
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

        if (State == EMonsterState.Attack || State == EMonsterState.Hit || State == EMonsterState.Death) normalized = 0f;

        _animator.SetFloat(HashSpeed, normalized, 0.1f, Time.deltaTime);
    }


    // 1. 함수는 한 가지 일만 잘해야 한다.
    // 2. 상태별 행동을 함수로 만든다.
    private void Idle()
    {
        // 대기하는 상태
        // Todo.Idle 애니메이션 실행

        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환: Idle -> Trace");
        }

        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _idleToPatrolDelay && _patrolPoints != null && _patrolPoints.Length > 0)
        {
            State = EMonsterState.Patrol;
            Debug.Log("상태 전환: Idle -> Patrol");
            _idleTimer = 0f;
        }
    }

    private void Trace()
    {
        // 플레이어를 쫓아가는 상태
        // Todo.Run 애니메이션 실행
        if (_player == null)
        {
            State = EMonsterState.Idle;
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        _agent.SetDestination(_playerTransform.position);
        // _agent.destination = _playerTransform.position;  위와 같은 기능을 하지만 함수로 쓰자

        if (distance <= AttackDistance)
        {
            State = EMonsterState.Attack;
        }
        else if (distance > ComebackDistance)
        {
            State = EMonsterState.Comeback;
            Debug.Log("상태 전환: Trace → Comeback");
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

            State = EMonsterState.Jump;
            return;
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
        State = EMonsterState.Trace;
    }

    private void Comeback()
    {
        // 제자리로 돌아간다.
        float distance = Vector3.Distance(transform.position, _spawnPosition);

        if (distance < ComebackPosition)
        {
            Debug.Log("상태 전환: Comeback → Idle");
            State = EMonsterState.Idle;
            return;
        }

        _agent.SetDestination(_spawnPosition);

        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환: Comeback -> Trace");
        }
    }

    private void Attack()
    {
        // 플레이어를 공격하는 상태
        // Todo.Attack 애니메이션 실행
        if (_player == null)
        {
            State = EMonsterState.Idle;
            return;
        }

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > AttackDistance)
        {
            State = EMonsterState.Trace;
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

                _playerStats.TryTakeDamage(damage.Value);
                Debug.Log("플레이어 공격!");
            }

            AttackTimer = 0f;
        }
    }

    public bool TryTakeDamage(Damage damage)
    {
        if (State == EMonsterState.Hit || State == EMonsterState.Death)
        {
            return false;
        }

        // 데미지를 받으면 데미지를 받은 위치에 혈흔 이펙트를 생성한다.
        // 그 이펙트는 몬스터를 따라다녀야 한다.
        GameObject bloodEffect = Instantiate(_bloodEffectPrefab, _player.transform.position, transform.rotation, transform);
        bloodEffect.transform.forward = damage.Normal;

        Health.Decrease(damage.Value);
        _knockbackDirection = (transform.position - _player.transform.position).normalized;

        if (Health.Value > 0f)
        {
            State = EMonsterState.Hit;
            Debug.Log("상태 전환: 어떤 상태 -> Hit");
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            State = EMonsterState.Death;
            Debug.Log("상태 전환: 어떤 상태 -> Death");
            StartCoroutine(Death_Coroutine());
        }

        return true;
    }

    private IEnumerator Hit_Coroutine()
    {
        // Todo.Hit 애니메이션 실행
        _animator.SetTrigger("Hit");

        _agent.isStopped = true;  // 이동 일시 정지
        _agent.ResetPath();  // 경로(목적지) 삭제

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
            State = EMonsterState.Trace;
        }
        else
        {
            State = EMonsterState.Idle;
        }
        StopCoroutine(Hit_Coroutine());
    }

    private IEnumerator Death_Coroutine()
    {
        // Todo.Death 애니메이션 실행
        _animator.SetTrigger("Death");

        _agent.isStopped = true;

        float timer = 0f;
        while (timer < KnockbackDuration)
        {
            _agent.Move(_knockbackDirection * KnockbackForce * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        DropCoins();
        Destroy(gameObject, DeathDuration);
    }

    public void DropCoins()
    {
        for (int i = 0; i < _coinAmount; i++)
        {
            GameObject coin = GoldPool.Instance.GetFromPool(transform.position,Quaternion.identity);

            Rigidbody rigidbody = coin.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Vector3 direction = new Vector3(Random.Range(-1f, 1f),1f,Random.Range(-1f, 1f)).normalized;

                rigidbody.AddForce(direction * _dropForce, ForceMode.Impulse);
            }
        }
    }

    private void Patrol()
    { 
        if (_player != null)
        { 
            float playerDistance = Vector3.Distance(transform.position, _player.transform.position);
            if (playerDistance <= DetectDistance) { State = EMonsterState.Trace; Debug.Log("상태 전환: Patrol -> Trace"); return; }
        }

        if (_patrolPoints == null || _patrolPoints.Length == 0)
        {
            State = EMonsterState.Idle;
            return;
        }
        
        Transform targetPoint = _patrolPoints[_currentPatrolIndex];
        _agent.SetDestination(targetPoint.position);
        float distance = Vector3.Distance(transform.position, targetPoint.position);
        if (distance <= _patrolArriveDistance)
        {
            _patrolWaitTimer += Time.deltaTime;
            if (_patrolWaitTimer >= _patrolWaitTime)
            {
                _currentPatrolIndex++;
                if (_currentPatrolIndex >= _patrolPoints.Length)
                {
                    _currentPatrolIndex = 0;
                }
                _patrolWaitTimer = 0f;
            }
        }
    }
}
