using TMPro;
using UnityEngine;

public class UI_GoldTotal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    [Header("표시 옵션")]
    [SerializeField] private string _goldText = "골드: ";

    public void SetGold(int totalGold)
    {
        if (_text == null) return;
        _text.text = $"{_goldText}{totalGold:N0}";
    }
}
