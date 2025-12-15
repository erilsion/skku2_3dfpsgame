using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    [Header("현재 폭탄 개수")]
    public int CurrentBombCount { get; private set; }

    [Header("최대 폭탄 개수")]
    public int MaxBombCount = 5;

    [SerializeField] private UI_Bomb _uiBomb;


    private void Awake()
    {
        Instance = this;
    }

    public void AddBomb()
    {
        CurrentBombCount++;
        if (CurrentBombCount > MaxBombCount)
        {
            CurrentBombCount = MaxBombCount;
        }
        _uiBomb.UpdateUI();
    }
    public void RemoveBomb()
    {
        CurrentBombCount--;
        _uiBomb.UpdateUI();
    }
}
