using UnityEngine;

public class BulletPool : PoolBase
{
    public static BulletPool Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    // 총알 생성용 편의 메서드
    public GameObject SpawnBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = GetFromPool(position, rotation);

        return bullet;
    }
}
