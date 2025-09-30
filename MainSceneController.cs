using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainSceneController : MonoBehaviour
{
    public Button button2; // ���ֱ� ��ư
    public ParticleSystem rainParticleSystem; // �� ��ƼŬ �ý��� �߰�
    private bool quizStarted = false; // ���� ���� ���� üũ
    private int waterButtonClickCount = 0; // ���ֱ� ��ư Ŭ�� Ƚ��

    void Start()
    {
        // �ʱ� ����: ��ư2�� ��Ȱ��ȭ�մϴ�.
        button2.interactable = false;

        // ���� ��� Ȯ��
        CheckQuizResult();

        // �� ��ƼŬ �ý��� ��Ȱ��ȭ
        rainParticleSystem.Stop();
    }

    void CheckQuizResult()
    {
        int quizResult = PlayerPrefs.GetInt("QuizResult", -1);

        // ���� ��� �ʱ�ȭ
        PlayerPrefs.SetInt("QuizResult", -1);

        if (quizResult == 1)
        {
            Debug.Log("���� ���� ��ũ��Ʈ ����");
            ExecuteSuccessScript();
        }
        else if (quizResult == 0)
        {
            Debug.Log("���� ���� ��ũ��Ʈ ����");
            ExecuteFailureScript();
        }
    }

    void ExecuteSuccessScript()
    {
        // ���� �� ������ ��ũ��Ʈ ������ ���⿡ �ۼ�

        // ��ư2 Ȱ��ȭ
        button2.interactable = true;

        // ��ư2�� Ŭ�� �̺�Ʈ ������ �߰�
        button2.onClick.AddListener(OnWaterButtonClicked);
    }

    void ExecuteFailureScript()
    {
        // ���� �� ������ ��ũ��Ʈ ������ ���⿡ �ۼ�
    }

    public void StartQuiz()
    {
        SceneManager.LoadScene("QuizScene");
        quizStarted = true; // ��� ���۵��� ǥ��
    }

    private void OnWaterButtonClicked()
    {
        // ���ֱ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
        Debug.Log("���ֱ� ��ư Ŭ��!");

        // Ŭ�� Ƚ�� ����
        waterButtonClickCount++;

        // �� ��ƼŬ �ý��� Ȱ��ȭ
        rainParticleSystem.Play();

        // 3�� �� �� ��ƼŬ �ý��� ����
        StartCoroutine(StopRainParticleSystemAfterDelay(3f));

        // Ŭ�� Ƚ���� 2ȸ�� ��� ��ư ��Ȱ��ȭ
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