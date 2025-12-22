using TMPro;
using UnityEngine;

public class UI_Click : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _leftClickCountTextUI;
    [SerializeField] private TextMeshProUGUI _rightClickCountTextUI;

    // 옵저버 패턴: 객체(주체자, Subject)의 데이터가 바뀔 때마다 주체자를 감시하는 객체에게 그 상태의 변경을 알려주는 패턴

    private void Start()
    {
        Refresh();

        // 구독 시작 (데이터 변경되면 Refresh로 호출해주세요 라고 등록)
        // 구독자들이 등록한 함수를 '콜백 함수'라고 한다. (어떤 이벤트가 발생하면 실행되는 함수를 콜백 함수라고 부른다.)
        ClickManager.Instance.OnDataChanged += Refresh;
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        _leftClickCountTextUI.text = $"왼쪽 클릭: {ClickManager.Instance.LeftClickCount}번";
        _rightClickCountTextUI.text = $"오른쪽 클릭: {ClickManager.Instance.RightClickCount}번";
    }
}
