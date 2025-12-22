using System;
using UnityEngine;

// 클릭 매니저의 역할: 게임에서 마우스 왼쪽 클릭과 오른쪽 클릭을 각각 몇번했는지 추적하는 클래스
public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance;

    private int _leftClickCount = 0;
    private int _rightClickCount = 0;
    private int _clickCount = 1;

    public int LeftClickCount => _leftClickCount;
    public int RightClickCount => _rightClickCount;

    // 클릭 매니저는 구독 함수 목록을 가지고 있고, 데이터가 변경될 때마다 그 함수들을 모두 호출해준다.
    public event Action OnDataChanged;

    // 세분화를 할 수 있다.
    // 너무 세분화할수록 읽어야 할 코드와 상황이 많아져서 힘들어진다.
    // public event Action OnLeftClickCountChanged;
    // public event Action OnRightClickCountChanged;

    // 레이어드 아키텍처: 시스템을 데이터/매니저/UI 계층으로 나누고, 의존성을 한 방향으로 강제함으로써
    // 좀 더 독립적인 구조를 지키는 것이다.이를 위해서 옵저버 패턴을 사용한다.


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _leftClickCount += _clickCount;

            OnDataChanged?.Invoke();
            // if (OnDataChanged != null) OnDataChanged(); 와 같다.
        }

        if (Input.GetMouseButtonDown(1))
        {
            _rightClickCount += _clickCount;

            OnDataChanged?.Invoke();
        }
    }
}
