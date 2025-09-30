using UnityEngine;
using UnityEngine.Networking;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�
using System.Collections;

public class ArduinoDataFetcher : MonoBehaviour
{
    private string url = "http://172.20.10.7"; // Arduino ������ IP �ּҷ� ��ü
    public TextMeshProUGUI sensorText; // TextMeshProUGUI�� ����

    void Start()
    {
        if (sensorText == null)
        {
            Debug.LogError("sensorText�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� TextMeshProUGUI UI ��Ҹ� �Ҵ��ϼ���.");
            return;
        }

        StartCoroutine(FetchDataFromArduino());
    }

    IEnumerator FetchDataFromArduino()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);

            // ��û�� ������ ������ ��ٸ�
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("������ �������� ����: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Arduino�κ����� ����: " + responseText); // ��ü ������ �α׷� ���
                ProcessResponse(responseText);
            }

            www.Dispose();

            yield return new WaitForSeconds(5); // 5�� ��� �� �ٽ� ��û
        }
    }

    void ProcessResponse(string response)
    {
        if (sensorText == null)
        {
            Debug.LogError("sensorText�� Inspector���� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        try
        {
            string temperature = ParseSensorData(response, "Temperature: ", " *C");
            string humidity = ParseSensorData(response, "Humidity: ", " %<br>");
            string soilMoisture = ParseSensorData(response, "Soil Moisture: ", " %<br>");
            string lightIntensity = ParseSensorData(response, "Light Level: ", "<br>");

            // ���� ������ ��ȿ�� �˻�
            if (int.TryParse(lightIntensity, out int lightValue))
            {
                if (lightValue == 1022)
                {
                    // 1022�� �ִ밪���� �����ϰ� 0���� ��ȯ (�ʿ信 ���� ����)
                    lightIntensity = "0";
                }
            }
            else
            {
                lightIntensity = "N/A"; // �Ľ� ���� �� "N/A" ó��
            }

            Debug.Log("����� �µ�: " + temperature + " ��C");
            Debug.Log("����� ����: " + humidity + " %");
            Debug.Log("����� ��� ����: " + soilMoisture + " %");
            Debug.Log("����� ����: " + lightIntensity );

            // UI ������Ʈ
            sensorText.text = "�µ�: " + temperature + " ��C\n����: " + humidity + " %\n��� ����: " + soilMoisture + " %\n��� : " + lightIntensity + " %";
        }
        catch (System.Exception e)
        {
            Debug.LogError("���� �Ľ� �� ���� �߻�: " + e.Message);
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
                Debug.Log($"�Ľ̵� ������ ({startToken}): {parsedValue}");
                return parsedValue;
            }
            else
            {
                Debug.LogError(endToken + "��(��) ã�� �� �����ϴ�.");
                return "N/A";
            }
        }
        else
        {
            Debug.LogError(startToken + "��(��) ã�� �� �����ϴ�.");
            return "N/A";
        }
    }
}