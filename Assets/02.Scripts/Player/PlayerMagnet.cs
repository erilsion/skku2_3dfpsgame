using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    [SerializeField] private Transform _player;

    private void Awake()
    {
        if (_player == null) _player = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Gold")) return;

        Gold gold = other.GetComponent<Gold>();
        if (gold != null)
        {
            gold.StartAttract(_player);
        }
    }
}
