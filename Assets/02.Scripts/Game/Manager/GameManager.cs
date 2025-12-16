using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [Header("텍스트 UI")]
    [SerializeField] private TextMeshProUGUI _stateTextUI;

    public event System.Action<EGameState> OnStateChanged;


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _stateTextUI.gameObject.SetActive(true);

        ChangeState(EGameState.Ready, forceNotify: true);
        _stateTextUI.text = "준비중...";

        StartCoroutine(StartToPlay_Coroutine());
    }

    private IEnumerator StartToPlay_Coroutine()
    {
        yield return new WaitForSeconds(2f);

        _stateTextUI.text = "시작!";

        yield return new WaitForSeconds(0.5f);

        ChangeState(EGameState.Playing);

        _stateTextUI.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        if (_state == EGameState.GameOver) return;  // 중복 방지

        _stateTextUI.gameObject.SetActive(true);
        _stateTextUI.text = "게임 오버!";
        ChangeState(EGameState.GameOver);
    }

    private void ChangeState(EGameState newState, bool forceNotify = false)
    {
        if (!forceNotify && _state == newState) return;
        _state = newState;
        OnStateChanged?.Invoke(_state);
    }
}
