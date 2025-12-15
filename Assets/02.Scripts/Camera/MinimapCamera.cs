using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Header("미니맵 카메라")]
    [SerializeField] private Camera _minimapCamera;
    [SerializeField] private Transform _target;
    [SerializeField] private float _offsetY = 10f;
    [SerializeField] private float _angleX = 90;

    [Header("줌 설정")]
    [SerializeField] private float _zoomStep = 2f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 20f;

    private void Awake()
    {
        if (_minimapCamera == null)
        {
            _minimapCamera = GetComponent<Camera>();
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = _target.position;
        Vector3 finalPosition = targetPosition + new Vector3(0f, _offsetY, 0f);

        transform.position = finalPosition;

        Vector3 targetAngle = _target.eulerAngles;
        targetAngle.x = _angleX;

        transform.eulerAngles = targetAngle;
    }

    public void ZoomIn()
    {
        SetZoom(_minimapCamera.orthographicSize - _zoomStep);
    }

    public void ZoomOut()
    {
        SetZoom(_minimapCamera.orthographicSize + _zoomStep);
    }

    private void SetZoom(float value)
    {
        _minimapCamera.orthographicSize = Mathf.Clamp(value, _minZoom, _maxZoom);
    }
}
