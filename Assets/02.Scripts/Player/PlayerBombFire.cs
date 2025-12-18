using System.Collections;
using UnityEngine;

public class PlayerBombFire : PlayStateListener
{
    // 마우스 오른쪽 버튼을 누르면 카메라(플레이어)가 바라보는 방향으로 폭탄을 던지고 싶다.

    [Header("발사 위치")]
    [SerializeField] private Transform _fireTransform;

    [Header("던질 힘")]
    [SerializeField] private float _throwPower = 15f;
    [SerializeField] private float _throwTime = 1.4f;

    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!IsPlaying) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (BombManager.Instance.CurrentBombCount >= BombManager.Instance.MaxBombCount)
            {
                Debug.Log("폭탄 최대 개수 도달!");
                return;
            }
            StartCoroutine(ThrowBomb_Coroutine());
        }
    }

    private IEnumerator ThrowBomb_Coroutine()
    {
        _animator.SetTrigger("Bomb");
        yield return new WaitForSeconds(_throwTime);

        GameObject bomb = BombPool.Instance.SpawnBomb(_fireTransform.position, Quaternion.identity);
        BombManager.Instance.AddBomb();
        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();

        rigidbody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);

        StopCoroutine(ThrowBomb_Coroutine());
    }
}
