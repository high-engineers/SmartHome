#include <dht11.h>
#include <ArduinoJson.h>
#include <Arduino.h>

#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClient.h>

#define MOTION 12     //d6
#define DHT11PIN 13   //d7
#define RELAY_OUT 15  //d8
#define BPIN A0

dht11 DHT11;
int bvalue;
float brightness;
float humidity;
float temperature;
float minBrightness;
float minHumidity;
float minTemperature;
float maxBrightness;
float maxHumidity;
float maxTemperature;
StaticJsonDocument<200> dataToServer;
StaticJsonDocument<200> dataFromServer;

ESP8266WiFiMulti WiFiMulti;
WiFiClient client;
HTTPClient http;
char* wifiSSID="UPC941F685";
char* wifiPassword = "Mdb3tkRjnh7d";

const unsigned long postDelay = 5000;
unsigned long postTime = 0;

void  getMeasurements() {
  dataToServer.clear();
  DHT11.read(DHT11PIN);
  bvalue = analogRead(BPIN);
  brightness = (bvalue/1024.0) * 100.0;
  brightness = 100.0 - brightness;
  humidity = (float)DHT11.humidity;
  temperature = (float)DHT11.temperature;
  dataToServer["brightness"] = brightness;
  dataToServer["humidity"] = humidity;
  dataToServer["temperature"] = temperature;

  serializeJsonPretty(dataToServer, Serial);
  Serial.println();
  delay(1000);
}

void setup() {
  pinMode(DHT11PIN, INPUT);
  pinMode(BPIN, INPUT);
  pinMode(MOTION, INPUT);
  pinMode(RELAY_OUT, OUTPUT);
  digitalWrite(RELAY_OUT, LOW);
  Serial.begin(115200);
  while(!Serial);
  minBrightness = 60.0;

  WiFi.mode(WIFI_STA);
  WiFiMulti.addAP(wifiSSID, wifiPassword);
  Serial.println("sada");
  while(!(WiFiMulti.run() == WL_CONNECTED)){
    Serial.println("Connecting to wifi...");
    WiFiMulti.addAP(wifiSSID, wifiPassword);
    delay(1000);
  }
  Serial.println("Connected!");
}

void bulbActivation(){
  if (minBrightness > brightness) {
    digitalWrite(RELAY_OUT, HIGH);
    Serial.println("ZAPALAM ŚWIATŁO");
  } else {
    digitalWrite(RELAY_OUT, LOW);
    Serial.println("GASZĘ ŚWIATŁO");
  }
}

void loop() {
  getMeasurements();
  bulbActivation();
//  getInfoFromServer();
//  if (millis() - postTime > postDelay){
//    postTime += postDelay;
//    postDataToServer();
//  }
  checkMotion();
}



void getInfoFromServer(){
  dataFromServer.clear();
   
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/values")) {


    Serial.print("[HTTP] GET...\n");
    // start connection and send HTTP header
    int httpCode = http.GET();

    // httpCode will be negative on error
    if (httpCode > 0) {
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTP] GET... code: %d\n", httpCode);

      // file found at server
      if (httpCode == HTTP_CODE_OK || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
        String payload = http.getString();
        Serial.println(payload);
        setUserVariables(payload);
//        Serial.println(minbrightness);
//        Serial.println(minHumidity);
//        Serial.println(minTemperature);
//        Serial.println(maxBrightness);
//        Serial.println(maxHumidity);
//        Serial.println(maxTemperature);
      }
    } else {
      Serial.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }

    http.end();
  } else {
    Serial.printf("[HTTP} Unable to connect\n");
  }
}

void postDataToServer(){
  getMeasurements();
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://my-json-server.typicode.com/Wmilosz/test/user")){
    http.addHeader("Content-Type", "application/json");
    String temp;
    serializeJsonPretty(dataToServer, temp);
    int httpCode = http.POST(temp);

    if (httpCode > 0) {
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTP] POST... code: %d\n", httpCode);

      if (httpCode == 201 || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
        String payload = http.getString();
        Serial.println(payload);
        Serial.println("SUCESS!");
      }
    } else {
      Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }
    http.end();
  } else {
    Serial.printf("[HTTP} Unable to connect\n");
  }
  
}

void setUserVariables(String data){
  deserializeJson(dataFromServer, data);
  minBrightness = dataFromServer["minBrightness"];
  minHumidity = dataFromServer["minHumidity"];
  minTemperature = dataFromServer["minTemperature"];
  maxBrightness = dataFromServer["maxBrightness"];
  maxHumidity = dataFromServer["maxHumidity"];
  maxTemperature = dataFromServer["maxTemperature"];
}

void checkMotion(){
  if (digitalRead(MOTION) == HIGH){
    Serial.println("RUCH!");
    delay(1000);
  }
}
