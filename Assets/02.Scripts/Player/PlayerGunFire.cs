using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    // 목표: 마우스 왼쪽 버튼을 누르면 카메라(플레이어)가 바라보는 방향으로 총알을 발사하고 싶다. (총알을 날리고 싶다.)

    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffectPrefab;

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼이 눌린다면
        if(Input.GetMouseButtonDown(0))
        {
            // 2. Ray를 생성하고 발사할 위치, 방향, 거리를 설정한다. (쏜다.)
            Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);

            // 3. RayCastHit(충돌한 대상의 정보)를 저장할 변수를 생성한다.
            RaycastHit hitInfo = new RaycastHit();

            // 4. 어떤 대상과 충돌했다면 피격 이펙트 표시
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {
                Debug.Log(hitInfo.transform.name);
                ParticleSystem hitEffect = Instantiate(_hitEffectPrefab, hitInfo.point, Quaternion.identity);
                hitEffect.transform.forward = hitInfo.normal;
                hitEffect.Play();
            }
        }
    }
}
