using UnityEngine;
using System;

public class TryCatchFinallyTest : MonoBehaviour
{
    public int Age; // 사람 나이

    // 예외: 런타임 중에 발생하는 오류 (참조, 나누기, 인덱스 범위 벗어나기 등등)

    // try-catch 문법: 예외를 처리하는 기본 문법

    private void Start()
    {
        if (Age < 0)
        {
            Debug.LogError("사람 나이는 0살 미만일 수 없습니다.");
            throw new Exception("사람 나이는 0살 미만일 수 없습니다.");  // DDD
        }


        // 아래 문법은 인덱스 범위를 벗어나므로 오류가 일어난다.
        // -> 다른 컴포넌트나 게임 오브젝트에도 영향을 줌으로써 프로그램이 정상적으로 동작 안할 수 있다.

        // 베스트: 알고리즘을 잘 처리하는 것

        int[] numbers = new int[32];

        try
        {
            // 예외가 발생할만한 코드 작성
            int index = 75; // 실제로는 내가 문제를 해결하기 위해 반복문이나 수식을 통해 얻은 인덱스
            numbers[index] = 1;
        }
        catch(Exception e)
        {
            Debug.Log(e); // 예외 정보 출력

            // 예외가 발생했을 때 처리해야할 코드 작성
            int index = numbers.Length - 1;
            numbers[index] = 1;
            Debug.Log("IndexOutOfRange 오류 발생, ㅇㅇㅇ를 찾아주세요.");
        }
        finally
        {
            // 선택사항: 정상이든 오류든 실행할 코드 작성
        }


        // Try-Catch 구문은 되도록이면 안 쓰는 것이 좋다.
        // 1. 성능 저하 (제일 큰 이유)
        // 2. 잘못된 알고리즘

        // 써야 하는 경우: 내가 제어할 수 없을 때
        // 제어할 수 없는 상황에서 나타나는 오류는 수십가지이므로 일일히 방어코드를 작성할 수 없다.
        // 1. 네트워크 접근 (로그인, 로그아웃, 서버 접속, DB 아이템 저장/불러오기)
        // 2. 파일 접근 (용량, 파일명, 권한 등등)
        // 3. DB 접근
    }
}
