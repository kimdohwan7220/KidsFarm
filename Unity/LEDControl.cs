using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LEDControl : MonoBehaviour
{
    public string arduinoIP = "172.20.10.7"; // 아두이노의 IP 주소로 변경
    public Button ledOnButton;
    public Button ledOffButton;

    void Start()
    {
        // 버튼에 클릭 이벤트 리스너 등록
        ledOnButton.onClick.AddListener(() => StartCoroutine(OnLedOnButtonClicked()));
        ledOffButton.onClick.AddListener(() => StartCoroutine(OnLedOffButtonClicked()));
    }

    IEnumerator OnLedOnButtonClicked()
    {
        ledOffButton.interactable = false; // LED Off 버튼 비활성화
        StartCoroutine(SendRequest("on")); // LED On 요청 보내기
        yield return new WaitForSeconds(4); // 4초 대기
        ledOffButton.interactable = true;  // LED Off 버튼 활성화
    }

    IEnumerator OnLedOffButtonClicked()
    {
        ledOnButton.interactable = false; // LED On 버튼 비활성화
        StartCoroutine(SendRequest("off")); // LED Off 요청 보내기
        yield return new WaitForSeconds(4); // 4초 대기
        ledOnButton.interactable = true;  // LED On 버튼 활성화
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
