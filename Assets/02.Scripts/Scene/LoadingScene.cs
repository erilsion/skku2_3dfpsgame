using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private TextMeshProUGUI _progressText;
    private int _hundred = 100;
    private float _ninetyPercent = 0.9f;


    private void Start()
    {
        // 비동기 -> 유저가 다른 일을 하는 동안 불러온다.
        StartCoroutine(LoadScene_Coroutine());
    }

    private IEnumerator LoadScene_Coroutine()
    {
        // 비동기 -> 유저가 다른 일을 하는 동안 불러온다.
        // 씬 로드 상황에 대한 데이터를 가지고 있는 객체를 반환한다.
        AsyncOperation ao = SceneManager.LoadSceneAsync("SampleScene");

        // 로드되는 씬의 모습이 화면에 안 보이게 한다.
        ao.allowSceneActivation = false;

        // 로드가 완료될 때까지
        while (!ao.isDone)
        { 
            _progressSlider.value = ao.progress;
            _progressText.text = $"{ao.progress * _hundred}%";

            if (ao.progress >= _ninetyPercent)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
