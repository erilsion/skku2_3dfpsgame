using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    private Monster _monster;
    [SerializeField] private GameObject _player;

    private void Awake()
    {
        if (_monster == null)
        {
            _monster = GetComponent<Monster>();
        }
    }

    public void PlayerAttack()
    {
        // PlayerStats player = _player.GetComponent<PlayerStats>;
        // player.TryTakeDamage(_monster.Damage);
    }
}
