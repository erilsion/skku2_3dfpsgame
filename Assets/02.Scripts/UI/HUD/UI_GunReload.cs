using UnityEngine;
using UnityEngine.UI;

public class UI_GunReload : MonoBehaviour
{
    [SerializeField] private Slider _reloadSlider;

    private void Awake()
    {
        _reloadSlider.minValue = 0f;
        _reloadSlider.maxValue = 1f;
        _reloadSlider.value = 1f;
    }

    public void StartReload()
    {
        _reloadSlider.value = 0f;
    }

    public void SetReloadProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        _reloadSlider.value = progress;
    }
}
