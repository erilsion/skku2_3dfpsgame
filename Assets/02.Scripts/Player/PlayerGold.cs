using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public int gold;

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log("현재 골드: " + gold);
    }
}
