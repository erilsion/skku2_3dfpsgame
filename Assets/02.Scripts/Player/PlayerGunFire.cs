using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerGunFire : PlayStateListener
{
    [Header("발사 위치")]
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private List<GameObject> _muzzleEffects;

    [Header("피격 이펙트")]
    [SerializeField] private ParticleSystem _hitEffect;

    [Header("쿨타임")]
    private float _fireTimer = 0f;
    private float _fireCooltime = 0.4f;
    private float _shootTime = 0.08f;

    [Header("재장전 시간")]
    [SerializeField] private float _reloadTime = 1.6f;
    private float _reloadTimer = 0f;
    private bool _isReloading = false;

    [Header("UI")]
    [SerializeField] private UI_GunReload _uiReloadBar;

    [Header("총알 개수 제한")]
    [SerializeField] private int _maxBullet = 30;
    private int _currentBullet = 0;
    private int _reserveBullet = 150;

    [Header("데미지")]
    [SerializeField] private int _damage = 10;

    private Animator _animator;

    [Header("라인 렌더러")]
    [SerializeField] private LineRenderer _tracer;
    [SerializeField] private float _maxRange = 80f;
    [SerializeField] private float _tracerTravelTime = 0.06f;
    [SerializeField] private float _tracerStayTime = 0.02f;
    [SerializeField] private float _tracerTailTime = 0.02f;
    [SerializeField] private float _startOffset = 0.1f;

    private Coroutine _tracerRoutine;

    [Header("조준 보정")]
    [SerializeField] private LayerMask _aimMask;
    [SerializeField] private float _aimMaxDistance = 200f;
    [SerializeField] private bool _flattenYInTopView = true;

    [Header("카메라 상태(선택)")]
    [SerializeField] private CameraFollow _cameraFollow;

    private void Awake()
    {
        _currentBullet = _maxBullet;
        _animator = GetComponentInChildren<Animator>();

        if (_tracer != null)
        {
            _tracer.positionCount = 2;
            _tracer.enabled = false;
        }
    }

    private void Update()
    {
        if (!IsPlaying) return;

        _fireTimer += Time.deltaTime;

        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;

            float progress = 1f - (_reloadTimer / _reloadTime);
            _uiReloadBar.SetReloadProgress(progress);

            if (_reloadTimer <= 0f)
            {
                ReloadFinish();
                _isReloading = false;
                _uiReloadBar.SetReloadProgress(1f);
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && _currentBullet < _maxBullet)
        {
            if (_reserveBullet <= 0) return;

            _isReloading = true;
            _reloadTimer = _reloadTime;
            _uiReloadBar.StartReload();
            return;
        }

        if (_fireTimer < _fireCooltime) return;

        if (Input.GetMouseButton(0))
        {
            if (Shoot()) StartCoroutine(MuzzleEffect_Coroutine());
        }
    }

    private bool Shoot()
    {
        if (_currentBullet <= 0) return false;

        _animator.SetTrigger("Attack");

        Vector3 aimPoint = GetAimPoint();
        Vector3 direction = (aimPoint - _fireTransform.position);

        if (_flattenYInTopView && IsTopView())
        {
            direction.y = 0f;
        }
        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = _fireTransform.forward;
        }
        direction.Normalize();

        Vector3 start = _fireTransform.position + direction * _startOffset;
        Ray ray = new Ray(_fireTransform.position, direction);

        bool isHit = Physics.Raycast(ray, out RaycastHit hitInfo, _maxRange);
        Vector3 end = isHit ? hitInfo.point : (_fireTransform.position + direction * _maxRange);

        PlayTracerTravel(start, end);

        _currentBullet--;

        if (isHit)
        {
            if (_hitEffect != null)
            {
                _hitEffect.transform.position = hitInfo.point;
                _hitEffect.transform.forward = hitInfo.normal;
                _hitEffect.Play();
            }

            if (hitInfo.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage);
            }
        }

        CameraRecoil.Instance.DoRecoil();
        _fireTimer = 0f;

        return true;
    }

    private Vector3 GetAimPoint()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            return _fireTransform.position + _fireTransform.forward * _aimMaxDistance;
        }

        Ray camRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        int mask = (_aimMask.value == 0) ? Physics.DefaultRaycastLayers : _aimMask;

        if (Physics.Raycast(camRay, out RaycastHit hit, _aimMaxDistance, mask))
        {
            return hit.point;
        }
        return camRay.origin + camRay.direction * _aimMaxDistance;
    }

    private bool IsTopView()
    {
        if (_cameraFollow == null) return false;
        return _cameraFollow.State == ECameraState.TopView;
    }

    private IEnumerator MuzzleEffect_Coroutine()
    {
        if (_muzzleEffects == null || _muzzleEffects.Count == 0) yield break;

        GameObject muzzleEffect = _muzzleEffects[Random.Range(0, _muzzleEffects.Count)];
        if (muzzleEffect == null) yield break;

        muzzleEffect.SetActive(true);
        yield return new WaitForSeconds(_shootTime);
        muzzleEffect.SetActive(false);
    }

    private void ReloadFinish()
    {
        int needAmmunition = _maxBullet - _currentBullet;
        int ammunitionToLoad = Mathf.Min(needAmmunition, _reserveBullet);

        _currentBullet += ammunitionToLoad;
        _reserveBullet -= ammunitionToLoad;

        Debug.Log($"재장전 완료! 탄창: {_currentBullet} | 예비탄: {_reserveBullet}");
    }

    private void PlayTracerTravel(Vector3 start, Vector3 end)
    {
        if (_tracer == null) return;

        if (_tracerRoutine != null) StopCoroutine(_tracerRoutine);
        _tracerRoutine = StartCoroutine(TracerTravel_Coroutine(start, end));
    }

    private IEnumerator TracerTravel_Coroutine(Vector3 start, Vector3 end)
    {
        _tracer.enabled = true;

        _tracer.SetPosition(0, start);
        _tracer.SetPosition(1, start);

        float headTravel = 0f;
        float tailTravel = 0f;

        float travel = Mathf.Max(0.0001f, _tracerTravelTime);
        float tailDelay = Mathf.Max(0.0f, _tracerTailTime);

        while (headTravel < 1f)
        {
            headTravel += Time.deltaTime / travel;
            headTravel = Mathf.Clamp01(headTravel);

            float headEase = Mathf.SmoothStep(0f, 1f, headTravel);

            if (tailDelay <= 0f)
            {
                tailTravel = headTravel;
            }
            else
            {
                float tailTime = Mathf.Clamp01((headTravel * travel - tailDelay) / travel);
                tailTravel = Mathf.SmoothStep(0f, 1f, tailTime);
            }

            Vector3 headPosition = Vector3.Lerp(start, end, headEase);
            Vector3 tailPosition = Vector3.Lerp(start, end, tailTravel);

            _tracer.SetPosition(0, tailPosition);
            _tracer.SetPosition(1, headPosition);

            yield return null;
        }

        yield return new WaitForSeconds(_tracerStayTime);

        _tracer.enabled = false;
        _tracerRoutine = null;
    }
}
