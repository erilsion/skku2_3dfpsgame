using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    // 마우스 오른쪽 버튼을 누르면 카메라(플레이어)가 바라보는 방향으로 폭탄을 던지고 싶다.

    [Header("발사 위치")]
    [SerializeField] private Transform _fireTransform;

    [Header("발사할 폭탄")]
    [SerializeField] private Bomb _bombPrefab;

    [Header("던질 힘")]
    [SerializeField] private float _throwPower = 15f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Bomb bomb = Instantiate(_bombPrefab, _fireTransform.position, Quaternion.identity);
            Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();

            rigidbody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
        }
    }
}
