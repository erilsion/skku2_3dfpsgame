using UnityEngine;

public class BombPool : PoolBase
{
    public static BombPool Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    // 폭탄 생성용 편의 메서드
    public GameObject SpawnBomb(Vector3 position, Quaternion rotation)
    {
        GameObject bomb = GetFromPool(position, rotation);

        return bomb;
    }
}

