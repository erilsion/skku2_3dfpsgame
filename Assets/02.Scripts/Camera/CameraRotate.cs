using UnityEngine;

// 마우스를 조작하면 카메라를 그 방향으로 회전시키는 스크립트
public class CameraRotate : PlayStateListener
{
    public float RotationSpeed = 300f;  // 초당 300도 회전

    // 게임 시작하면 y축이 0도에서 시작 -> 아래로 살짝 쳐다보면 -1도 -> 유니티는 359도로 인식 -> y축 최대 회전 각도 때문에 90도로 튐
    // 유니티는 0~360각도 체계이므로 우리가 따로 저장할 -360~360 체계로 누적할 변수
    private float _accumulationX = 0f;
    private float _accumulationY = 0f;


    private void Update()
    {
        if (!IsPlaying) return;

        if (!Input.GetMouseButton(1)) return; // 오른쪽 마우스 버튼 클릭 시에만 카메라 회전

        // 1. 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 2. 마우스 입력 값을 누적한 방향을 구한다.
        // 새로운 위치 = 이전 위치 + (속도 * 방향 * 시간)
        // 새로운 회전 = 이전 회전 + (속도 * 방향 * 시간)
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;
        _accumulationY += -mouseY * RotationSpeed * Time.deltaTime;

        _accumulationY = Mathf.Clamp(_accumulationY, -90f, 90f);  // Y축 회전 각도 제한

        // 3. 회전 방향으로 카메라 회전하기
        // 쿼터니언(사원수): 쓰는 이유는 짐벌락 현상 방지
        transform.eulerAngles = new Vector3(_accumulationY, _accumulationX);
    }
}
