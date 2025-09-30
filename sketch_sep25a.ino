#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include "WiFiEsp.h"
#include <DHT.h>
#include <Servo.h>

LiquidCrystal_I2C lcd(0x27, 16, 2);

// Hardware Serial이 없으면 SoftwareSerial 사용
#ifndef HAVE_HWSERIAL1
#include "SoftwareSerial.h"
SoftwareSerial Serial2(0, 1); // RX, TX
#endif

// Wi-Fi 네트워크 정보
char ssid[] = "doji";            // 네트워크 SSID (이름)
char pass[] = "123456788";       // 네트워크 비밀번호
int status = WL_IDLE_STATUS;    // Wi-Fi 라디오의 상태
int reqCount = 0;               // 수신한 요청 수

WiFiEspServer server(80);       // Wi-Fi 서버 포트 80으로 설정

// 센서 및 핀 설정
#define DHTPIN 12               // DHT 센서가 연결된 핀
#define DHTTYPE DHT11           // DHT 11 (DHT22 사용 시 DHT22로 변경)
DHT dht(DHTPIN, DHTTYPE);

#define SOIL_MOISTURE_PIN A1    // 토양 습도 센서 핀
#define LIGHT_SENSOR_PIN A0     // 조도 센서 핀
#define WATER_PUMP_PIN 31       // 물 펌프 핀
#define LED_PIN 4               // LED 핀
#define VENTILATION_MOTOR_PIN 32 // 환기 모터 핀

Servo ventilationServo;         // 서보 모터 객체 생성
#define SERVO_PIN 9             // 서보 모터 핀

void setup() {
  // 시리얼 통신 설정
  Serial.begin(115200);
  Serial2.begin(9600);
  WiFi.init(&Serial2);

  // Wi-Fi 쉴드가 있는지 확인
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi shield not present");
    while (true);
  }

  // Wi-Fi 네트워크에 연결 시도
  while (status != WL_CONNECTED) {
    Serial.print("Attempting to connect to WPA SSID: ");
    Serial.println(ssid);
    status = WiFi.begin(ssid, pass);
  }

  // 네트워크 연결 완료 메시지
  Serial.println("Connected to network");
  printWifiStatus();
  server.begin();               // 서버 시작
  dht.begin();                  // DHT 센서 시작

  // 핀 모드 설정
  pinMode(WATER_PUMP_PIN, OUTPUT);
  digitalWrite(WATER_PUMP_PIN, LOW);
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);
  pinMode(VENTILATION_MOTOR_PIN, OUTPUT);
  digitalWrite(VENTILATION_MOTOR_PIN, LOW);

  // 서보 모터 초기화 및 초기 위치 설정
  ventilationServo.attach(SERVO_PIN);
  ventilationServo.write(0);

  // LCD 초기화
  lcd.init();
  lcd.backlight();
  lcd.setCursor(0, 0);
  lcd.print("Server is on");
  lcd.setCursor(0, 1);
  lcd.print(WiFi.localIP());
}

void loop() {
  // 클라이언트가 있는지 확인
  WiFiEspClient client = server.available();
  if (client) {
    Serial.println("New client");
    boolean currentLineIsBlank = true;
    String currentLine = "";
    while (client.connected()) {
      if (client.available()) {
        char c = client.read();
        Serial.write(c);
        currentLine += c;

        // 빈 줄이 있으면 응답 전송
        if (c == '\n' && currentLineIsBlank) {
          Serial.println("Sending response");

          // DHT 센서에서 온도 및 습도 읽기
          float humidity = dht.readHumidity();
          float temperature = dht.readTemperature();

          // 센서 읽기 실패 시 메시지 출력
          if (isnan(humidity) || isnan(temperature)) {
            Serial.println("Failed to read from DHT sensor!");
            break;
          }

          // 토양 습도 및 조도 센서 읽기
          int soilMoistureValue = analogRead(SOIL_MOISTURE_PIN);
          float soilMoisturePercent = map(soilMoistureValue, 0, 1023, 100, 0);
          int lightSensorValue = analogRead(LIGHT_SENSOR_PIN);
          int lightLevel = map(lightSensorValue, 0, 1023, 100, 0);

          // HTTP 응답 작성 및 전송
          client.print(
            "HTTP/1.1 200 OK\r\n"
            "Content-Type: text/html\r\n"
            "Connection: close\r\n"
            "Refresh: 5\r\n"
            "\r\n");
          client.print("<!DOCTYPE HTML>\r\n");
          client.print("<html>\r\n");
          client.print("<h1>Kid's Farm Sensor Information!</h1>\r\n");
          client.print("Requests received: ");
          client.print(++reqCount);
          client.print("<br>\r\n");
          client.print("Temperature: ");
          client.print(temperature);
          client.print(" *C<br>\r\n");
          client.print("Humidity: ");
          client.print(humidity);
          client.print(" %<br>\r\n");
          client.print("Soil Moisture: ");
          client.print(soilMoisturePercent);
          client.print(" %<br>\r\n");
          client.print("Light Level: ");
          client.print(lightLevel);
          client.print("<br>\r\n");
          client.print("</html>\r\n");
          break;
        }

        // 줄 바꿈 확인
        if (c == '\n') {
          currentLineIsBlank = true;
        } else if (c != '\r') {
          currentLineIsBlank = false;
        }
      }

      // URL 요청 처리
      if (currentLine.endsWith("GET /pump/on")) {
        digitalWrite(WATER_PUMP_PIN, HIGH);
        delay(3000);
        digitalWrite(WATER_PUMP_PIN, LOW);
      }
      if (currentLine.endsWith("GET /pump/off")) {
        digitalWrite(WATER_PUMP_PIN, LOW);
      }
      if (currentLine.endsWith("GET /led/on")) {
        digitalWrite(LED_PIN, HIGH);
      }
      if (currentLine.endsWith("GET /led/off")) {
        digitalWrite(LED_PIN, LOW);
      }
      if (currentLine.endsWith("GET /ventilation/on")) {
        digitalWrite(VENTILATION_MOTOR_PIN, HIGH);
        ventilationServo.write(90);
      }
      if (currentLine.endsWith("GET /ventilation/off")) {
        digitalWrite(VENTILATION_MOTOR_PIN, LOW);
        ventilationServo.write(0);
      }
    }

    client.stop();
    Serial.println("Client disconnected");
  }
}

// Wi-Fi 상태 출력
void printWifiStatus() {
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);
  Serial.print("To see this page in your browser, type http://");
  Serial.println(ip);
  Serial.println();
}