using System;
using UnityEngine;
using UnityEngine.AI;

// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다.
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMove : PlayStateListener
{
    [Serializable]
    public class MoveConfig
    {
        public float Gravity;
        public float RunStamina;
        public float JumpStamina;
        public float DoubleJumpStamina;

        [Header("Auto Move")]
        public float AutoStopDistance = 0.2f;
        public float AutoTurnSpeed = 12f;
    }

    public MoveConfig _config;


    private CharacterController _controller;
    private PlayerStats _stats;
    private NavMeshAgent _agent;

    private float _yVelocity = 0f;   // 중력에 의해 누적될 y값 변수
    private bool _canDoubleJump = false;

    private bool _isAutoMoving = false;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stats = GetComponent<PlayerStats>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.autoBraking = true;
    }

    private void Update()
    {
        if (!IsPlaying) return;

        if (_controller.isGrounded)
        {
            _canDoubleJump = false;
            if (_yVelocity < 0) _yVelocity = -1f;  // 지면에 붙여주는 안정 처리
        }

        // 0. 중력을 누적한다.
        _yVelocity += _config.Gravity * Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            if (TrySetAutoMove()) _isAutoMoving = true;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        bool hasManualInput = Mathf.Abs(x) > 0.01f || Mathf.Abs(y) > 0.01f;

        if (hasManualInput && _isAutoMoving)
        {
            CancelAutoMove();
        }

        HandleJump();

        if (_isAutoMoving && !hasManualInput)
        {
            AutoMove();
        }
        else
        {
            ManualMove(x, y);
        }
    }

    private void ManualMove(float x, float z)
    {
        Vector3 direction = new Vector3(x, 0, z);
        direction.Normalize();

        // 카메라 기준 이동
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0f;

        float moveSpeed = GetMoveSpeed();

        Vector3 move = direction * moveSpeed;
        move.y = _yVelocity;

        _controller.Move(move * Time.deltaTime);
    }

    private void AutoMove()
    {
        // agent가 계산한 다음 코너 방향으로 이동
        if (_agent.pathPending)
            return;

        // 도착 판정
        float remain = Vector3.Distance(transform.position, _agent.destination);
        if (remain <= _config.AutoStopDistance || (_agent.hasPath == false))
        {
            CancelAutoMove();
            return;
        }

        // 현재 위치 동기화(경로 추적 안정화)
        _agent.nextPosition = transform.position;

        Vector3 desired = _agent.desiredVelocity; // "다음 방향"을 알려줌
        desired.y = 0f;

        float moveSpeed = GetMoveSpeed();
        Vector3 move = desired.normalized * moveSpeed;
        move.y = _yVelocity;

        _controller.Move(move * Time.deltaTime);

        // 자동이동 시 진행 방향으로 회전(선택)
        if (desired.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desired);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _config.AutoTurnSpeed);
        }
    }

    private float GetMoveSpeed()
    {
        float moveSpeed = _stats.MoveSpeed.Value;
        if (Input.GetKey(KeyCode.LeftShift) && _stats.Stamina.TryConsume(_config.RunStamina * Time.deltaTime))
            moveSpeed = _stats.RunSpeed.Value;
        return moveSpeed;
    }

    private void CancelAutoMove()
    {
        _isAutoMoving = false;
        _agent.ResetPath();
    }

    private bool TrySetAutoMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 999f))
            return false;

        // NavMesh 위의 가까운 점으로 보정
        if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2.0f, NavMesh.AllAreas))
        {
            _agent.SetDestination(navHit.position);
            return true;
        }

        return false;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            _yVelocity = _stats.JumpPower.Value;
        }
        else if (_canDoubleJump == false && !_controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump") && _stats.Stamina.TryConsume(_config.DoubleJumpStamina))
            {
                _canDoubleJump = true;
                _yVelocity = _stats.JumpPower.Value;
            }
        }
    }
}
