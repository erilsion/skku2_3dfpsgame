using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : MonoBehaviour
{
    [Header("줌인 / 줌아웃 버튼")]
    [SerializeField] private Button _zoomInButton;
    [SerializeField] private Button _zoomOutButton;

    [Header("미니맵 카메라")]
    [SerializeField] private MinimapCamera _minimapCamera;

    private void Awake()
    {
        _zoomInButton.onClick.AddListener(OnZoomInClicked);
        _zoomOutButton.onClick.AddListener(OnZoomOutClicked);
    }

    private void OnDestroy()
    {
        _zoomInButton.onClick.RemoveListener(OnZoomInClicked);
        _zoomOutButton.onClick.RemoveListener(OnZoomOutClicked);
    }

    private void OnZoomInClicked()
    {
        _minimapCamera.ZoomIn();
    }

    private void OnZoomOutClicked()
    {
        _minimapCamera.ZoomOut();
    }
}
