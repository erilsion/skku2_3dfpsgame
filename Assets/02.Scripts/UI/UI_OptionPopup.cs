using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_OptionPopup : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _gameExitButton;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        Hide();

        // 콜백 함수: 어떤 이벤트가 일어나면 자동으로 호출되는 함수
        _continueButton.onClick.AddListener(GameContinue);
        _retryButton.onClick.AddListener(GameRetry);
        _gameExitButton.onClick.AddListener(GameExit);
    }

    // 함수란 한 가지 기능만 해야되고, 그 기능이 무엇을 하는지(의도, 결과) 나타나는 이름을 가져야 된다.
    // '~했을 때'라는 이름은 기능이 아니라 '언제 호출되는지'를 나타내는 이름이다.
    private void GameContinue()
    {
        GameManager.Instance.Continue();

        Hide();
    }

    private void GameRetry()
    {
        GameManager.Instance.Retry();
    }

    private void GameExit()
    {
        // 여기서 Application.Quit();를 할 수 있지만, 중요한 일은 관리자에게 맡긴다.
        GameManager.Instance.Quit();
    }
}
