using UnityEngine;

public class EliteMonsterAttack : MonoBehaviour
{
    [Header("몬스터 관련 옵션")]
    [SerializeField] private EliteMonster _eliteMonster;

    [Header("플레이어 관련 옵션")]
    [SerializeField] private GameObject _player;
    private PlayerStats _playerStats;

    private void Awake()
    {
        _eliteMonster = GetComponentInParent<EliteMonster>();
        _playerStats = _player.GetComponent<PlayerStats>();
    }


    public void PlayerAttack()
    {
        if (_playerStats == null) return;

        if (_playerStats != null)
        {
            Damage damage = new Damage()
            {
                Value = _eliteMonster.Damage,
                HitPoint = transform.position
            };

            _playerStats.TryTakeDamage(damage.Value);

        }
    }
}
