using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerGunFire : PlayStateListener
{
    // 목표: 마우스 왼쪽 버튼을 누르면 카메라(플레이어)가 바라보는 방향으로 총알을 발사하고 싶다. (총알을 날리고 싶다.)

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
            if (_currentBullet >= _maxBullet) return;
            if (_reserveBullet <= 0) return;

            _isReloading = true;
            _reloadTimer = _reloadTime;
            _uiReloadBar.StartReload();
            return;
        }

        if (_fireTimer < _fireCooltime) return;

        // 1. 마우스 왼쪽 버튼이 눌린다면
        if (Input.GetMouseButton(0))
        {
            Shoot();
            StartCoroutine(MuzzleEffect_Coroutine());
        }
    }

    private void Shoot()
    {
        if (_currentBullet <= 0) return;

        _animator.SetTrigger("Attack");

        Vector3 direction = Camera.main.transform.forward;
        Vector3 start = _fireTransform.position + direction * _startOffset;

        // 2. Ray를 생성하고 발사할 위치, 방향, 거리를 설정한다. (쏜다.)
        Ray ray = new Ray(_fireTransform.position, direction);

        // 3. RayCastHit(충돌한 대상의 정보)를 저장할 변수를 생성한다.
        RaycastHit hitInfo = new RaycastHit();

        // 4. 어떤 대상과 충돌했다면 피격 이펙트 표시
        bool isHit = Physics.Raycast(ray, out hitInfo, _maxRange);
        Vector3 end = isHit ? hitInfo.point : (start + direction * _maxRange);
        PlayTracerTravel(start, end);

        if (isHit)
        {
            Debug.Log(hitInfo.transform.name);

            _currentBullet--;

            _hitEffect.transform.position = hitInfo.point;
            _hitEffect.transform.forward = hitInfo.normal;

            _hitEffect.Play();

            if (hitInfo.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TryTakeDamage(_damage);
            }
        }
        else
        {
            _currentBullet--;
        }

        CameraRecoil.Instance.DoRecoil();
        _fireTimer = 0f;
    }

    // 파티클 생성과 플레이 방식
    // 1. Instantiate 방식 (+ 풀링) -> 한 화면에 여러가지 수정 후 여러 개 그릴 경우. 새로 생성 (메모리, CPU)
    // 2. 하나를 캐싱해두고 Play    -> 인스펙터 설정 그대로 그릴 경우. 한 화면에 한번만 그릴 경우. 단점: 재실행이므로 기존 게 삭제
    // 3. 하나를 캐싱해두고 Emit    -> 인스펙터 설정을 수정한 후 그릴 경우. 한 화면에 위치만 수정 후 여러 개 그릴 경우

    // ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
    // emitParams.position = hitInfo.point;
    // emitParams.rotation3D = Quaternion.LookRotation(hitInfo.normal).eulerAngles;

    // _hitEffect.Emit(emitParams, 1);    커스텀할 정보, 분출 횟수

    private IEnumerator MuzzleEffect_Coroutine()
    {
        GameObject muzzleEffect = _muzzleEffects[Random.Range(0, _muzzleEffects.Count)];

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
