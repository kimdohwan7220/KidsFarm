using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WaterPumpController : MonoBehaviour
{
    public string arduinoIP = "172.20.10.7"; // �Ƶ��̳��� IP �ּҷ� ����
    public Button pumpOnButton;
    public Button pumpOffButton;

    void Start()
    {
        // ��ư�� Ŭ�� �̺�Ʈ ������ ���
        pumpOnButton.onClick.AddListener(() => StartCoroutine(SendRequest("on")));
        pumpOffButton.onClick.AddListener(() => StartCoroutine(SendRequest("off")));
    }

    IEnumerator SendRequest(string pumpState)
    {
        string url = $"http://{arduinoIP}/pump/{pumpState}";
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
