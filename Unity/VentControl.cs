using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VentControl : MonoBehaviour
{
    public string arduinoIP = "172.20.10.7"; // 아두이노의 IP 주소로 변경
    public Button ventOnButton;
    public Button ventOffButton;

    void Start()
    {
        // 버튼에 클릭 이벤트 리스너 등록
        ventOnButton.onClick.AddListener(() => StartCoroutine(OnVentOnButtonClicked()));
        ventOffButton.onClick.AddListener(() => StartCoroutine(OnVentOffButtonClicked()));
    }

    IEnumerator OnVentOnButtonClicked()
    {
        ventOffButton.interactable = false; // Vent Off 버튼 비활성화
        StartCoroutine(SendRequest("on")); // Vent On 요청 보내기
        yield return new WaitForSeconds(4); // 4초 대기
        ventOffButton.interactable = true;  // Vent Off 버튼 활성화
    }

    IEnumerator OnVentOffButtonClicked()
    {
        ventOnButton.interactable = false; // Vent On 버튼 비활성화
        StartCoroutine(SendRequest("off")); // Vent Off 요청 보내기
        yield return new WaitForSeconds(4); // 4초 대기
        ventOnButton.interactable = true;  // Vent On 버튼 활성화
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
