using System.Collections;
using UnityEngine;

public class CoroutineExample1 : MonoBehaviour
{
    // 코드를 '동기'적으로 실행하는 게 아니라 '비동기'적으로 실행하고 싶다.
    // 동기: 이전 코드가 실행 완료된 다음에 그 다음 코드를 실행하는 것
    // 비동기: 이전 코드가 실행 완료되지 않아도 그 다음 코드를 실행하는 것 (병렬)
    // 유니티에서는 '비동기' 방식을 지원하기 위해 "코루틴"이라는 기능을 제공한다.

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Test1();
            StartCoroutine(Test2_Coroutine());  // 코루틴: 협력(Co) + 루틴(Routine). 협력 동작
            Test3();
        }
    }

    private void Test1()
    {
        Debug.Log("Test1");
    }

    private IEnumerator Test2_Coroutine()
    {
        Debug.Log("Test2");

        int sum = 0;
        for(int i = 0; i < 1000000; i++)
        {
            // yield 키워드를 이용하면 코루틴 함수의 실행을 중단하고 이어할 수 있다.
            yield return null;                    // 다음 프레임까지 쉰다.
            yield return new WaitForSeconds(3f);  // 3초 쉰다.
            yield break;                          // 코루틴을 끝낸다.

            for (int j = 0; j < 100; j++)
            {
                sum += (i * j);
            }
        }

        Debug.Log(sum);
    }

    private void Test3()
    {
        Debug.Log("Test3");
    }
}
