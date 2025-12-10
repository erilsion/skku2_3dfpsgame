using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    public static CameraRecoil Instance;

    [Header("반동 설정")]
    [SerializeField] private float _recoilAmount = 6f;
    [SerializeField] private float _recoilSpeed = 18f;
    [SerializeField] private float _returnSpeed = 12f;

    private Vector3 _currentRecoil;
    private Vector3 _targetRecoil;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        _targetRecoil = Vector3.Lerp(_targetRecoil, Vector3.zero, _returnSpeed * Time.deltaTime);
        _currentRecoil = Vector3.Lerp(_currentRecoil, _targetRecoil, _recoilSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(_currentRecoil);
    }

    public void DoRecoil()
    {
        float randomX = Random.Range(-1f, 1f);
        _targetRecoil += new Vector3(-_recoilAmount, randomX * _recoilAmount * 0.5f, 0f);
    }
}
