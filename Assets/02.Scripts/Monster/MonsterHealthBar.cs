using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Monster))]
public class MonsterHealthBar : MonoBehaviour
{
    [Header("몬스터 컴포넌트")]
    private Monster _monster;

    [Header("체력 게이지바 관련")]
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _gaugeImage;
    private float _lastHealth = -1f;

    [Header("카메라")]
    [SerializeField] private Camera _mainCamera;


    private void Awake()
    {
        _monster = gameObject.GetComponent<Monster>();
    }

    private void LateUpdate()
    {
        // UI가 알고 있는 몬스터 체력값과 다를 경우에만 fillAmount를 수정한다.
        if (_lastHealth != _monster.Health.Value)
        {
            _gaugeImage.fillAmount = _monster.Health.Value / _monster.Health.MaxValue;
        }

        // 빌보드 기법: 카메라의 위치와 회전에 상관없이 항상 정면을 바라보게 하는 기법
        _healthBarTransform.forward = _mainCamera.transform.forward;
    }
}
