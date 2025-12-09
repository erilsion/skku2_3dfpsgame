using UnityEngine;
using UnityEngine.UI;

// 플레이어의 '스탯'을 관리하는 컴포넌트
public class PlayerStats : MonoBehaviour
{
    // 도메인: 특정 분야의 지식

    [Header("스태미나")]  // 소모 가능한 스탯
    public ConsumableStat Stamina;
    [SerializeField] private Slider _staminaSlider;
    //private float _staminaDecreasePerSecond = 20f;
    //private float _staminaRecoverPerSecond = 10f;

    [Header("체력")]  // 소모 가능한 스탯
    public ConsumableStat Health;
    [SerializeField] private Slider _healthSlider;

    [Header("스탯")]
    public ValueStat Damage;
    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;
    public ValueStat JumpPower;


    // 스태미나, 체력 스탯 관련 코드 (회복, 소모, 업그레이드...)

    private void Start()
    {
        Health.Initialize();
        Stamina.Initialize();
    }

    private void Update()
    {
        SprintMovement();
    }
    void SprintMovement()
    {
        //bool canSprint = _stamina.Value > 0f;
        bool isSprintKeyPressed = Input.GetKey(KeyCode.LeftShift);

        //bool isSprinting = isSprintKeyPressed && canSprint;

        //if (isSprinting)
        {
            //_stamina.Decrease(_staminaDecreasePerSecond * Time.deltaTime);

            //if (_playerMove != null)
            //    _playerMove.SetRunMode();
        }
        //else
        {
            //_stamina.Increase(_staminaRecoverPerSecond * Time.deltaTime);

            //if (_playerMove != null)
            //    _playerMove.SetWalkMode();
        }
    }

    public bool TryUseStamina(float amount)
    {
        //if (Stamina.Value < amount)
        //    return false;

        Stamina.Decrease(amount);
        return true;
    }
}
