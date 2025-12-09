using UnityEngine;

// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다.
[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("이동 속도")]
    private float _playerSpeed = 7f;
    public float MoveSpeed = 7f;
    public float RunSpeed = 20f;

    [Header("점프력")]
    public float JumpPower = 5f;

    [Header("2단 점프")]
    private float _doubleJumpCost = 20f;
    private bool _hasDoubleJumped = false;

    [Header("중력")]
    public float Gravity = -9.81f;
    private float _yVelocity = 0f;  // 중력에 의해 누적될 y값 변수

    [Header("컨트롤러")]
    private CharacterController _controller;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _playerStats = GetComponent<PlayerStats>();
        _playerSpeed = MoveSpeed;
    }

    private void Update()
    {
        // 0. 중력을 누적한다.
        _yVelocity += Gravity * Time.deltaTime;

        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // 2. 입력에 따른 방향 구하기
        // 현재는 유니티 세상의 절대적인 방향이 기준 (글로벌/월드 좌표계)
        // 내가 원하는 것: 캐릭터(카메라)가 쳐다보는 방향이 기준 (로컬 좌표계)

        // 2_1. 글로벌 좌표 방향을 구한다.
        Vector3 direction = new Vector3(x, 0, y);
        direction.Normalize();

        // 점프
        if (Input.GetButtonDown("Jump") && _controller.isGrounded)
        {
            _yVelocity = JumpPower;
        }

        // 2_2. 캐릭터(카메라)가 쳐다보는 방향으로 변환한다. (월드 -> 로컬)
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = _yVelocity;  // 중력 적용

        // 3. 방향으로 이동시키기
        _controller.Move(direction * _playerSpeed * Time.deltaTime);
    }
    public void SetRunMode()
    {
        _playerSpeed = RunSpeed;
    }
    public void SetWalkMode()
    {
        _playerSpeed = MoveSpeed;
    }
}
