using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    public static CameraRecoil Instance;

    [Header("반동 설정")]
    [SerializeField] private float recoilAmount = 6f;
    [SerializeField] private float recoilSpeed = 18f;
    [SerializeField] private float returnSpeed = 12f;

    private Vector3 currentRecoil;
    private Vector3 targetRecoil;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, recoilSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(currentRecoil);
    }

    public void DoRecoil()
    {
        float randomX = Random.Range(-1f, 1f);
        targetRecoil += new Vector3(-recoilAmount, randomX * recoilAmount * 0.5f, 0f);
    }
}
