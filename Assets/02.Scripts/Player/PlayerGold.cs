using System;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public int Gold { get; private set; }

    public event Action<int, int> OnGoldGained;
    [SerializeField] private UI_GoldTotal _totalUI;

    private void Start()
    {
        RefreshUI();
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        Gold += amount;

        RefreshUI();
        OnGoldGained?.Invoke(amount, Gold);
    }

    private void RefreshUI()
    {
        if (_totalUI != null)
        {
            _totalUI.SetGold(Gold);
        }
    }
}
