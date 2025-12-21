using UnityEngine;

public class CoinPool : PoolBase
{
    public static CoinPool Instance;

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }
}
