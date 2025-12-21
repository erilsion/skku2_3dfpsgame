using UnityEngine;

public class CoinPool : PoolBase
{
    public static CoinPool Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }
}
