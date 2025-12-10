using UnityEngine;
using UnityEngine.UI;

public class UI_Bomb : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private Text _bombCountText;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        _bombCountText.text = $"폭탄: {BombManager.Instance.MaxBombCount - BombManager.Instance.CurrentBombCount} / {BombManager.Instance.MaxBombCount}";
    }
}
