using UnityEngine;
using UnityEngine.Networking;
using TMPro; // TextMeshPro 네임스페이스 추가
using System.Collections;

public class ArduinoDataFetcher : MonoBehaviour
{
    private string url = "http://172.20.10.7"; // Arduino 서버의 IP 주소로 교체
    public TextMeshProUGUI sensorText; // TextMeshProUGUI로 변경

    void Start()
    {
        if (sensorText == null)
        {
            Debug.LogError("sensorText가 할당되지 않았습니다. Inspector에서 TextMeshProUGUI UI 요소를 할당하세요.");
            return;
        }

        StartCoroutine(FetchDataFromArduino());
    }

    IEnumerator FetchDataFromArduino()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);

            // 요청을 보내고 응답을 기다림
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("데이터 가져오기 실패: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Arduino로부터의 응답: " + responseText); // 전체 응답을 로그로 출력
                ProcessResponse(responseText);
            }

            www.Dispose();

            yield return new WaitForSeconds(5); // 5초 대기 후 다시 요청
        }
    }

    void ProcessResponse(string response)
    {
        if (sensorText == null)
        {
            Debug.LogError("sensorText가 Inspector에서 할당되지 않았습니다!");
            return;
        }

        try
        {
            string temperature = ParseSensorData(response, "Temperature: ", " *C");
            string humidity = ParseSensorData(response, "Humidity: ", " %<br>");
            string soilMoisture = ParseSensorData(response, "Soil Moisture: ", " %<br>");
            string lightIntensity = ParseSensorData(response, "Light Level: ", "<br>");

            // 조도 데이터 유효성 검사
            if (int.TryParse(lightIntensity, out int lightValue))
            {
                if (lightValue == 1022)
                {
                    // 1022는 최대값으로 간주하고 0으로 변환 (필요에 따라 조정)
                    lightIntensity = "0";
                }
            }
            else
            {
                lightIntensity = "N/A"; // 파싱 실패 시 "N/A" 처리
            }

            Debug.Log("추출된 온도: " + temperature + " °C");
            Debug.Log("추출된 습도: " + humidity + " %");
            Debug.Log("추출된 토양 습도: " + soilMoisture + " %");
            Debug.Log("추출된 조도: " + lightIntensity );

            // UI 업데이트
            sensorText.text = "온도: " + temperature + " °C\n습도: " + humidity + " %\n토양 습도: " + soilMoisture + " %\n밝기 : " + lightIntensity + " %";
        }
        catch (System.Exception e)
        {
            Debug.LogError("응답 파싱 중 오류 발생: " + e.Message);
        }
    }

    string ParseSensorData(string response, string startToken, string endToken)
    {
        int startIndex = response.IndexOf(startToken);
        if (startIndex != -1)
        {
            startIndex += startToken.Length;
            int endIndex = response.IndexOf(endToken, startIndex);
            if (endIndex != -1)
            {
                string parsedValue = response.Substring(startIndex, endIndex - startIndex).Trim();
                Debug.Log($"파싱된 데이터 ({startToken}): {parsedValue}");
                return parsedValue;
            }
            else
            {
                Debug.LogError(endToken + "을(를) 찾을 수 없습니다.");
                return "N/A";
            }
        }
        else
        {
            Debug.LogError(startToken + "을(를) 찾을 수 없습니다.");
            return "N/A";
        }
    }
}
