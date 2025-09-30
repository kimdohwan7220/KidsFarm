using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LEDControl : MonoBehaviour
{
    public string arduinoIP = "172.20.10.7"; // �Ƶ��̳��� IP �ּҷ� ����
    public Button ledOnButton;
    public Button ledOffButton;

    void Start()
    {
        // ��ư�� Ŭ�� �̺�Ʈ ������ ���
        ledOnButton.onClick.AddListener(() => StartCoroutine(OnLedOnButtonClicked()));
        ledOffButton.onClick.AddListener(() => StartCoroutine(OnLedOffButtonClicked()));
    }

    IEnumerator OnLedOnButtonClicked()
    {
        ledOffButton.interactable = false; // LED Off ��ư ��Ȱ��ȭ
        StartCoroutine(SendRequest("on")); // LED On ��û ������
        yield return new WaitForSeconds(4); // 4�� ���
        ledOffButton.interactable = true;  // LED Off ��ư Ȱ��ȭ
    }

    IEnumerator OnLedOffButtonClicked()
    {
        ledOnButton.interactable = false; // LED On ��ư ��Ȱ��ȭ
        StartCoroutine(SendRequest("off")); // LED Off ��û ������
        yield return new WaitForSeconds(4); // 4�� ���
        ledOnButton.interactable = true;  // LED On ��ư Ȱ��ȭ
    }

    IEnumerator SendRequest(string ledState)
    {
        string url = $"http://{arduinoIP}/led/{ledState}";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {www.error}");
            }
            else
            {
                Debug.Log($"Response: {www.downloadHandler.text}");
            }
        }
    }
}
