using UnityEngine;
using DG.Tweening;

// 목표를 따라다니는 카메라

public class CameraFollow : PlayStateListener
{
    public ECameraState State = ECameraState.FPS;

    public Transform Target;

    [Header("기본 각도")]
    [SerializeField] private float _pitch = 0f;

    [Header("TPS")]
    [SerializeField] private Vector3 _tpsOffset = new Vector3(0, 1, -6);

    [Header("탑뷰")]
    [SerializeField] private Vector3 _topOffset = new Vector3(0, 2, -8);
    [SerializeField] private float _topPitch = 50f;

    [Header("전환 옵션")]
    [SerializeField] private float _switchDuration = 0.4f;

    private bool _isTweening = false;
    private float _currentPitch;
    private Tween _pitchTween;
    private Tween _transitionTween;


    private void OnEnable()
    {
        _currentPitch = _pitch;
        EnterState(State, tween: false);
    }

    private void Update()
    {
        if (!IsPlaying) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            ChangeState(NextState(State));
        }
    }

    private void LateUpdate()
    {
        if (!IsPlaying) return;
        if (Target == null || _isTweening) return;

        FollowState(State);
    }

    private ECameraState NextState(ECameraState current)  // T를 누를 때마다 카메라 시점 전환
    {
        return current switch
        {
            ECameraState.FPS => ECameraState.TPS,
            ECameraState.TPS => ECameraState.TopView,
            ECameraState.TopView => ECameraState.FPS,
            _ => ECameraState.FPS
        };
    }
    private void ChangeState(ECameraState next)
    {
        if (Target == null) return;
        if (State == next) return;

        State = next;
        EnterState(State, tween: true);
    }

    private void EnterState(ECameraState state, bool tween)
    {
        transform.DOKill();
        _pitchTween?.Kill();
        _transitionTween?.Kill();

        float startPitch = _currentPitch;
        float targetPitch = state switch
        {
            ECameraState.FPS => _pitch,
            ECameraState.TPS => _pitch,
            ECameraState.TopView => _topPitch,
            _ => _pitch
        };

        float fixedYaw = transform.eulerAngles.y;

        Vector3 startPos = transform.position;

        if (!tween)
        {
            _isTweening = false;
            _currentPitch = targetPitch;
            ApplyPoseDuringTween(state, fixedYaw);
            return;
        }

        _isTweening = true;

        _transitionTween = DOVirtual.Float(0f, 1f, _switchDuration, t =>
        {
            _currentPitch = Mathf.Lerp(startPitch, targetPitch, t);

            transform.rotation = Quaternion.Euler(_currentPitch, fixedYaw, 0f);

            Vector3 desiredPos = GetDesiredPosition(state);

            transform.position = Vector3.Lerp(startPos, desiredPos, t);
        })
        .SetEase(Ease.InOutSine)
        .OnComplete(() => _isTweening = false);
    }

    private Vector3 GetDesiredPosition(ECameraState state)
    {
        switch (state)
        {
            case ECameraState.FPS:
                return Target.position;

            case ECameraState.TPS:
                return Target.position + transform.rotation * _tpsOffset;

            case ECameraState.TopView:
                return Target.position + transform.rotation * _topOffset;

            default:
                return Target.position;
        }
    }

    private void ApplyPoseDuringTween(ECameraState state, float yaw)
    {
        transform.rotation = Quaternion.Euler(_currentPitch, yaw, 0f);

        switch (state)
        {
            case ECameraState.FPS:
                transform.position = Target.position;
                break;

            case ECameraState.TPS:
                transform.position = Target.position + transform.rotation * _tpsOffset;
                break;

            case ECameraState.TopView:
                transform.position = Target.position + transform.rotation * _topOffset;
                break;
        }
    }

    private void FollowState(ECameraState state)
    {
        float yaw = transform.eulerAngles.y;

        switch (state)
        {
            case ECameraState.FPS:
                transform.position = Target.position;
                transform.rotation = Quaternion.Euler(_currentPitch, yaw, 0f);
                break;

            case ECameraState.TPS:
                transform.rotation = Quaternion.Euler(_currentPitch, yaw, 0f);
                transform.position = Target.position + transform.rotation * _tpsOffset;
                break;

            case ECameraState.TopView:
                transform.rotation = Quaternion.Euler(_currentPitch, yaw, 0f);
                transform.position = Target.position + transform.rotation * _topOffset;
                break;
        }
    }

    private (Vector3 position, Quaternion rotation, bool applyRotation) GetTargetPose(ECameraState state)
    {
        switch (state)
        {
            case ECameraState.FPS:
                return (Target.position, transform.rotation, false);

            case ECameraState.TPS:
                return (Target.position + transform.rotation * _tpsOffset, transform.rotation, false);

            case ECameraState.TopView:
                {
                    float yaw = transform.eulerAngles.y;
                    Quaternion rot = Quaternion.Euler(_topPitch, yaw, 0f);
                    Vector3 pos = Target.position + rot * _topOffset;
                    return (pos, rot, true);
                }

            default:
                return (Target.position, transform.rotation, false);
        }
    }
}

