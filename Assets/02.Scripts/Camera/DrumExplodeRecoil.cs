using UnityEngine;

public class DrumExplodeRecoil : MonoBehaviour
{
    [SerializeField] private CameraRecoil _cameraRecoil;
    [SerializeField] private Drum _drum;

    private void OnEnable()
    {
        if (_drum != null) _drum.OnExploded += HandleExploded;
    }

    private void OnDisable()
    {
        if (_drum != null) _drum.OnExploded -= HandleExploded;
    }

    private void HandleExploded(Drum drum)
    {
        if (_cameraRecoil != null)
            _cameraRecoil.DoRecoil();
    }
}
