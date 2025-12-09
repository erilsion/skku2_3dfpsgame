using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    public int CurrentBombCount { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddBomb() => CurrentBombCount++;
    public void RemoveBomb() => CurrentBombCount--;
}
