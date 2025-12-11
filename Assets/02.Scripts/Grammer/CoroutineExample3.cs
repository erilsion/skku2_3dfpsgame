using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class CoroutineExample3 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 1. 코루틴도 함수이므로 인자를 넘겨줄 수 있다.
        StartCoroutine(Sequence_Coroutine());
    }

    private IEnumerator Sequence_Coroutine()
    {
        // 여러 코루틴을 연속해서 실행할 경우 중첩 코루틴을 사용하지 말고 시퀀스 방식으로 해결하자.
        yield return StartCoroutine(Ready_Coroutine(1f));
        yield return StartCoroutine(Start_Coroutine(1f));
        yield return StartCoroutine(End_Coroutine(1f));
    }

    private IEnumerator Ready_Coroutine(float second)
    {
        yield return new WaitForSeconds(second);
        Debug.Log($"{second}초 대기");

        // 2. 코루틴 내부에서 다른 코루틴을 호출할 수 있다. (중첩 코루틴은 사용 X)
        //StartCoroutine(Start_Coroutine(second));
    }

    private IEnumerator Start_Coroutine(float second)
    {
        Debug.Log($"{second}초 대기");
        yield return new WaitForSeconds(second);

        Debug.Log("시작!");

        StartCoroutine(End_Coroutine(second));
    }

    private IEnumerator End_Coroutine(float second)
    {
        Debug.Log($"{second}초 대기");
        yield return new WaitForSeconds(second);
    }
}
