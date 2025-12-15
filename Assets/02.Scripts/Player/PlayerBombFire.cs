using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    // 마우스 오른쪽 버튼을 누르면 카메라(플레이어)가 바라보는 방향으로 폭탄을 던지고 싶다.

    [Header("발사 위치")]
    [SerializeField] private Transform _fireTransform;

    [Header("던질 힘")]
    [SerializeField] private float _throwPower = 15f;

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (BombManager.Instance.CurrentBombCount >= BombManager.Instance.MaxBombCount)
            {
                Debug.Log("폭탄 최대 개수 도달!");
                return;
            }

            GameObject bomb = BombPool.Instance.SpawnBomb(_fireTransform.position, Quaternion.identity);
            BombManager.Instance.AddBomb();
            Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();

            rigidbody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
        }
    }
}
