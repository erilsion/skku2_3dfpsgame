using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [Header("텍스트 UI")]
    [SerializeField] private TextMeshProUGUI _stateTextUI;

    [SerializeField] private UI_OptionPopup _optionPopupUI;

    public event System.Action<EGameState> OnStateChanged;


    private void Awake()
    {
        _instance = this;
        LockCursor();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();

            _optionPopupUI.Show();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;

        UnlockCursor();
    }

    public void Continue()
    {
        Time.timeScale = 1;

        LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Retry()
    {
        // 씬 재시작
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        // 게임 종료 Application.Quit(); - 빌드 상태에서만 유효하다.
        // 1. 데이터 저장
        // 2. 유저를 붙잡기도 함
        // 게임 종료 전 필요한 로직을 실행한다.

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
