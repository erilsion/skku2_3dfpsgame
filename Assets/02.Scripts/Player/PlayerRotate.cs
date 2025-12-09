using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 200f;  // 초당 200도 회전
    private float _accumulationX = 0f;

    private void Update()
    {
        if (!Input.GetMouseButton(1)) return;

        float mouseX = Input.GetAxis("Mouse X");
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0f, _accumulationX);
    }
}
