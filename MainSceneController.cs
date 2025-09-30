using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainSceneController : MonoBehaviour
{
    public Button button2; // 물주기 버튼
    public ParticleSystem rainParticleSystem; // 비 파티클 시스템 추가
    private bool quizStarted = false; // 퀴즈 시작 여부 체크
    private int waterButtonClickCount = 0; // 물주기 버튼 클릭 횟수

    void Start()
    {
        // 초기 설정: 버튼2를 비활성화합니다.
        button2.interactable = false;

        // 퀴즈 결과 확인
        CheckQuizResult();

        // 비 파티클 시스템 비활성화
        rainParticleSystem.Stop();
    }

    void CheckQuizResult()
    {
        int quizResult = PlayerPrefs.GetInt("QuizResult", -1);

        // 퀴즈 결과 초기화
        PlayerPrefs.SetInt("QuizResult", -1);

        if (quizResult == 1)
        {
            Debug.Log("퀴즈 성공 스크립트 실행");
            ExecuteSuccessScript();
        }
        else if (quizResult == 0)
        {
            Debug.Log("퀴즈 실패 스크립트 실행");
            ExecuteFailureScript();
        }
    }

    void ExecuteSuccessScript()
    {
        // 성공 시 실행할 스크립트 내용을 여기에 작성

        // 버튼2 활성화
        button2.interactable = true;

        // 버튼2에 클릭 이벤트 리스너 추가
        button2.onClick.AddListener(OnWaterButtonClicked);
    }

    void ExecuteFailureScript()
    {
        // 실패 시 실행할 스크립트 내용을 여기에 작성
    }

    public void StartQuiz()
    {
        SceneManager.LoadScene("QuizScene");
        quizStarted = true; // 퀴즈가 시작됨을 표시
    }

    private void OnWaterButtonClicked()
    {
        // 물주기 버튼 클릭 시 호출되는 메서드
        Debug.Log("물주기 버튼 클릭!");

        // 클릭 횟수 증가
        waterButtonClickCount++;

        // 비 파티클 시스템 활성화
        rainParticleSystem.Play();

        // 3초 후 비 파티클 시스템 중지
        StartCoroutine(StopRainParticleSystemAfterDelay(3f));

        // 클릭 횟수가 2회일 경우 버튼 비활성화
        if (waterButtonClickCount >= 1)
        {
            button2.interactable = false;
        }
    }

    private IEnumerator StopRainParticleSystemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rainParticleSystem.Stop();
    }
}