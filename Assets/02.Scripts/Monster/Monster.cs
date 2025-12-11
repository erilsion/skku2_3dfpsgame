using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
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

    public EMonsterState State = EMonsterState.Idle;
    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;

    public float MoveSpeed = 5f;
    public float AttackSpeed = 2f;
    public float AttackTimer = 0f;

    public float DetectDistance = 4f;
    public float AttackDistance = 1.5f;

    private void Update()
    {
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

            case EMonsterState.Hit:
                Hit();
                break;

            case EMonsterState.Death:
                Death();
                break;
        }
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
    }

    private void Trace()
    {
        // 플레이어를 쫓아가는 상태
        // Todo.Run 애니메이션 실행

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        // 1. 방향을 구한다.
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        // 2. 방향을 따라 이동한다.
        _controller.Move(direction * MoveSpeed * Time.deltaTime);
        if (distance <= AttackDistance)
        {
            State = EMonsterState.Attack;
        }
    }

    private void Comeback()
    {
        // 제자리로 돌아간다.
    }

    private void Attack()
    {
        // 플레이어를 공격하는 상태

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > AttackDistance)
        {
            State = EMonsterState.Trace;
            return;
        }

        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackSpeed)
        {
            Debug.Log("플레이어 공격!");
            AttackTimer = 0f;
        }
    }

    private void Hit()
    {
        // 피격당한다.
    }

    private void Death()
    {
        // 죽는다.
    }
}
