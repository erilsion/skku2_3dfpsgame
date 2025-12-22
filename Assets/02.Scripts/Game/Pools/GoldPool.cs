using UnityEngine;

public class GoldPool : PoolBase
{
    public static GoldPool Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }
}
