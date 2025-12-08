using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]  // 이 컴포넌트를 무조건 달게 해주는 어트리뷰트
public class ErrorTest1 : MonoBehaviour
{
    // 오류: 프로그램이 비정상적으로 동작하게 하는 문제
    // 예외: 프로그램이 실행 중 발생하고, 개발자가 예상하고 처리할 수 있는 문제

    // 1. 문법 오류 (컴파일 오류): 문법에 맞지 않는 코드나 오타로 인해 발생 -> 대부분 IDE가 잡아준다.
    // 2. 런타임 오류: 프로그램 실행 중 발생하는 오류 (에디터 콘솔창에 비교적 명확하게 출력) -> 테스트를 진행하면서 잡아주면 된다.
    // 3. 알고리즘 or 휴먼 오류 or AI 오류: 주어진 문제에 대해 잘못된 해석이나 구현으로 내가 원하지 않는 결과물이 나오는 오류
    //    ㄴ 가장 해결하기 어렵다. -> 공부/자료수집/분석 + 많은 경험을 통해 오류를 찾아내고 해결하는 능력 키우기

    // 유니티에서 런타임에(플레이) 주로 나타나는 오류(예외)

    // 1. NullReferenceException
    // 사용하고자 하는 객체가 null 값일 때 그 객체의 필드나 메서드에 접근하려고 하면 발생

    // 2. MisssingComponentException
    // 사용하고자 하는 컴포넌트가 null일 때

    private void Start()
    {
        // 1. NullReferenceException
        // 초기화 시에 null을 검사하는 방어코드 작성하기
        Rigidbody2D rigidbody2D = null;
        if (rigidbody2D == null)
        {
            // 적절한 처리 (AddComponent, 오류 로깅)
        }
        Debug.Log(rigidbody2D.linearVelocity);

        // 2. MisssingComponentException
        Rigidbody2D rigidbody2D2 = GetComponent<Rigidbody2D>();
        Debug.Log(rigidbody2D2.linearVelocity);
    }
}
