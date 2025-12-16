using UnityEngine;

public abstract class PlayStateListener : MonoBehaviour
{
    protected bool IsPlaying { get; private set; }

    private bool _subscribed;

    protected virtual void Start()
    {
        TrySubscribe();
    }

    protected virtual void OnDestroy()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (_subscribed) return;
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnStateChanged += HandleGameStateChanged;
        _subscribed = true;

        // 현재 상태 즉시 반영
        HandleGameStateChanged(GameManager.Instance.State);
    }

    private void Unsubscribe()
    {
        if (!_subscribed) return;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleGameStateChanged;
        }

        _subscribed = false;
    }

    private void HandleGameStateChanged(EGameState state)
    {
        IsPlaying = (state == EGameState.Playing);
    }
}
