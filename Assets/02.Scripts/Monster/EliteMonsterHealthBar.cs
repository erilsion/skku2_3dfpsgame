using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EliteMonster))]
public class EliteMonsterHealthBar : MonoBehaviour
{
    [Header("몬스터 컴포넌트")]
    private EliteMonster _monster;

    [Header("체력 게이지바 관련")]
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _gaugeImage;
    private float _lastHealth = -1f;

    [Header("카메라")]
    [SerializeField] private Camera _mainCamera;


    private void Awake()
    {
        _monster = gameObject.GetComponent<EliteMonster>();
    }

    private void LateUpdate()
    {
        float hp = _monster.Health.Value;
        if (_lastHealth != hp)
        {
            _gaugeImage.fillAmount = hp / _monster.Health.MaxValue;
            _lastHealth = hp;
        }

        _healthBarTransform.forward = _mainCamera.transform.forward;
    }
}
