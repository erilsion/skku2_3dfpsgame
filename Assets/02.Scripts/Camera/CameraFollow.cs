using UnityEngine;
using DG.Tweening;

// 목표를 따라다니는 카메라

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public Vector3 TpsOffset = new Vector3(0, 4, -8);
    private float _switchDuration = 0.4f;

    private bool _isFPS = false;
    private bool _isTweening = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isFPS = !_isFPS;
            SwitchCameraMode();
        }
    }

    private void LateUpdate()
    {
        if (Target == null || _isTweening) return;

        Vector3 offset;

        if (_isFPS)
        {
            transform.position = Target.position;
        }
        else
        {
            offset = TpsOffset;
            transform.position = Target.position + transform.rotation * offset;
        }
    }

    private void SwitchCameraMode()
    {
        if (Target == null) return;

        _isTweening = true;

        Vector3 targetPosition;

        if (_isFPS)
        {
            targetPosition = Target.position;
        }
        else
        {
            targetPosition = Target.position + transform.rotation * TpsOffset;
        }

        // 기존 Tween이 있다면 끊기지 않도록
        transform.DOKill();

        transform.DOMove(targetPosition, _switchDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _isTweening = false;
            });
    }
}

