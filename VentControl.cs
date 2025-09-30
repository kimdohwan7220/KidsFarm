using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VentControl : MonoBehaviour
{
    public string arduinoIP = "172.20.10.7"; // �Ƶ��̳��� IP �ּҷ� ����
    public Button ventOnButton;
    public Button ventOffButton;

    void Start()
    {
        // ��ư�� Ŭ�� �̺�Ʈ ������ ���
        ventOnButton.onClick.AddListener(() => StartCoroutine(OnVentOnButtonClicked()));
        ventOffButton.onClick.AddListener(() => StartCoroutine(OnVentOffButtonClicked()));
    }

    IEnumerator OnVentOnButtonClicked()
    {
        ventOffButton.interactable = false; // Vent Off ��ư ��Ȱ��ȭ
        StartCoroutine(SendRequest("on")); // Vent On ��û ������
        yield return new WaitForSeconds(4); // 4�� ���
        ventOffButton.interactable = true;  // Vent Off ��ư Ȱ��ȭ
    }

    IEnumerator OnVentOffButtonClicked()
    {
        ventOnButton.interactable = false; // Vent On ��ư ��Ȱ��ȭ
        StartCoroutine(SendRequest("off")); // Vent Off ��û ������
        yield return new WaitForSeconds(4); // 4�� ���
        ventOnButton.interactable = true;  // Vent On ��ư Ȱ��ȭ
    }

    IEnumerator SendRequest(string ventState)
    {
        string url = $"http://{arduinoIP}/ventilation/{ventState}";
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
