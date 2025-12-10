using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    // 목표: 마우스 왼쪽 버튼을 누르면 카메라(플레이어)가 바라보는 방향으로 총알을 발사하고 싶다. (총알을 날리고 싶다.)

    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼이 눌린다면
        if(Input.GetMouseButton(0))
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

                // 파티클 생성과 플레이 방식
                // 1. Instantiate 방식 (+ 풀링) -> 한 화면에 여러가지 수정 후 여러 개 그릴 경우. 새로 생성 (메모리, CPU)
                // 2. 하나를 캐싱해두고 Play    -> 인스펙터 설정 그대로 그릴 경우. 한 화면에 한번만 그릴 경우. 단점: 재실행이므로 기존 게 삭제
                // 3. 하나를 캐싱해두고 Emit    -> 인스펙터 설정을 수정한 후 그릴 경우. 한 화면에 위치만 수정 후 여러 개 그릴 경우

                _hitEffect.transform.position = hitInfo.point;
                _hitEffect.transform.forward = hitInfo.normal;

                _hitEffect.Play();

                // ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
                // emitParams.position = hitInfo.point;
                // emitParams.rotation3D = Quaternion.LookRotation(hitInfo.normal).eulerAngles;

                // _hitEffect.Emit(emitParams, 1);    커스텀할 정보, 분출 횟수
            }
        }
    }
}
