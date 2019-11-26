#include <dht11.h>
#include <ArduinoJson.h>
#include <Arduino.h>

#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClient.h>
#include <WiFiManager.h>
#include <EEPROM.h>
#include "EEPROMAnything.h"

#define MOTION_IN 12  //d6
#define DHT11PIN 13   //d7
#define RELAY_OUT 15  //d8
#define BUZZER_OUT 4  //d2
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
int motion;
int lightBulbState;
int alarmState;
int alarm;
bool motionBulb;
String wifiIP;
//ids
String homeID;
String lightBulbID;
String motionID;
String brightnessID;
String humidityID;
String temperatureID;
String alarmID;
//rooms
String lightBulbRoomID;
String motionRoomID;
String brightnessRoomID;
String humidityRoomID;
String temperatureRoomID;
String alarmRoomID;
String allRoomsIDs[6];
String uniqueRoomsIDs[2];

struct ComponentState{
  String Id;
  bool State = false;
};

ComponentState componentsState[2];

StaticJsonDocument<200> dataToServer;
StaticJsonDocument<200> dataFromServer;
StaticJsonDocument<200> measurement;

ESP8266WiFiMulti WiFiMulti;
WiFiClient client;
HTTPClient http;

const unsigned long postDelay = 5000;
unsigned long postTime = 0;

void getUniqueRoomsIDs(String rooms[]) {
  uniqueRoomsIDs[0] = rooms[0];
  int j = 0;
  for(int i = 1; i < 6; i++){
    if(rooms[i] != uniqueRoomsIDs[j]){
      j++;
      uniqueRoomsIDs[j] = rooms[i];
    }
  }
}

void  getMeasurements() {
  dataToServer.clear();
  DHT11.read(DHT11PIN);
  bvalue = analogRead(BPIN);
  brightness = (bvalue/1024.0) * 100.0;
  brightness = 100.0 - brightness;
  humidity = (float)DHT11.humidity;
  temperature = (float)DHT11.temperature;
  motion = digitalRead(MOTION_IN);
  
  dataToServer["brightness"] = brightness;
  dataToServer["humidity"] = humidity;
  dataToServer["temperature"] = temperature;

  serializeJsonPretty(dataToServer, Serial);
  Serial.println();
  delay(50);
}

void setup() {
  pinMode(DHT11PIN, INPUT);
  pinMode(BPIN, INPUT);
  pinMode(MOTION_IN, INPUT);
  pinMode(RELAY_OUT, OUTPUT);
  pinMode(BUZZER_OUT, OUTPUT);
  digitalWrite(RELAY_OUT, LOW);
  digitalWrite(BUZZER_OUT, LOW);
  Serial.begin(115200);
  while(!Serial);
  Serial.println("START");
  minBrightness = 60.0;
  WiFiManager wifiManager;
  Serial.println("Connecting ...");
  wifiManager.autoConnect("SmartHome");
  /*WiFi.mode(WIFI_STA);
  WiFiMulti.addAP(wifiSSID, wifiPassword);
  Serial.println("sada");
  while(!(WiFiMulti.run() == WL_CONNECTED)){
    Serial.println("Connecting to wifi...");
    WiFiMulti.addAP(wifiSSID, wifiPassword);
    delay(1000);
  }*/
  Serial.println("Connected!");
  wifiIP = WiFi.localIP().toString();
  alarm = true;
  motionBulb = true;
  motion = LOW;
}

void alarmActivation(){
  if (alarmState == 1 && (digitalRead(MOTION_IN) == HIGH)) {
    //turn on alarm
    Serial.println("ALARM ON!");
    digitalWrite(BUZZER_OUT, HIGH);
    delay(50);
  } else {
    //turn off alarm
    Serial.println("ALARM OFF!");
    digitalWrite(BUZZER_OUT, LOW);
    delay(50);
  }
}

void bulbActivation(){
  if (minBrightness > brightness || (motion == HIGH && motionBulb == true)) {
    digitalWrite(RELAY_OUT, HIGH);
    lightBulbState = 1;
    Serial.println("ZAPALAM ŚWIATŁO");
    delay(50);
  } else {
    digitalWrite(RELAY_OUT, LOW);
    lightBulbState = 0;
    Serial.println("GASZĘ ŚWIATŁO");
    delay(50);
  }
}

void loop() {
 getMeasurements();
 alarmActivation();
 bulbActivation();
 //getInfoFromServer();
//  if (millis() - postTime > postDelay){
//    postTime += postDelay;
//    postDataToServer();
//  }
  checkMotion();
}



void getInfoFromServer(){
  dataFromServer.clear();
   
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://my-json-server.typicode.com/WMilosz/test/user")) {


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

void getAllComponentsRoomsIDs(){
  motionRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("motion"));
  brightnessRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("brightness"));
  humidityRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("humidity"));
  temperatureRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("temperature"));
  lightBulbRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("lightBulb"));
  alarmRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("alarm"));
}

String getComponentRoomIDbyName(String componentName){
  if( componentName == "lightBulb"){
    return lightBulbID;
  } else if (componentName == "motion") {
    return motionID;
  } else if (componentName == "brightness") {
    return brightnessID;
  } else if (componentName == "humidity") {
    return humidityID;
  } else if(componentName == "temperature") {
    return temperatureID;
  } else if(componentName == "alarm") {
    return alarmID;
  } else {
    return "";
  }
  
}

void getComponentsStatesAllRooms(){
  for (int i =0; i < 2; i++){
    getComponentsStates(uniqueRoomsIDs[i]);
  }
}

void postComponentsReadingsAllRooms(){
  getMeasurements();
  String data;
  for (int i =0; i < 2; i++){
    measurement.clear();
    measurement["SensorId"] = motionID;
    measurement["Reading"] = motion;
    serializeJsonPretty(measurement, data);
    postComponentReadings(uniqueRoomsIDs[i], data);
    measurement.clear();
    measurement["SensorId"] = brightnessID;
    measurement["Reading"] = brightness;
    serializeJsonPretty(measurement, data);
    postComponentReadings(uniqueRoomsIDs[i], data);
    measurement.clear();
    measurement["SensorId"] = humidityID;
    measurement["Reading"] = humidity;
    serializeJsonPretty(measurement, data);
    postComponentReadings(uniqueRoomsIDs[i], data);
    measurement.clear();
    measurement["SensorId"] = temperatureID;
    measurement["Reading"] = temperature;
    serializeJsonPretty(measurement, data);
    postComponentReadings(uniqueRoomsIDs[i], data);
    measurement.clear();
    measurement["SensorId"] = lightBulbID;
    measurement["Reading"] = lightBulbState;
    serializeJsonPretty(measurement, data);
    postComponentReadings(uniqueRoomsIDs[i], data);
  }
}

void postComponentReadings(String roomID, String data){
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/components/collect?smartHomeEntityId=" + homeID + "&roomId=" + roomID)){
    http.addHeader("Content-Type", "application/json");
    int httpCode = http.POST(data);

    if (httpCode > 0) {
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTP] POST... code: %d\n", httpCode);

      if (httpCode == 200 || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
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

void writeStates(){
  if(componentsState[0].Id == lightBulbID){
    lightBulbState = componentsState[0].State;
    alarmState = componentsState[1].State;
  } else {
    lightBulbState = componentsState[1].State;
    alarmState = componentsState[0].State;
  }
}

void getComponentsStates(String roomID){
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/components?smartHomeEntityId=" + homeID + "&roomId=" + roomID)) {


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
        DynamicJsonDocument data(1024);
        deserializeJson(data, payload);
        JsonArray root = data.to<JsonArray>();
        int i = 0;
        for (JsonObject item : root) {
          componentsState[i].Id = item["Id"].as<String>();
          componentsState[i].State = item["State"].as<String>();
          i++;
        }
        //componentsState = data["ComponentArduinoInfo"];
        writeStates();
      }
    } else {
      Serial.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }
    http.end();
  } else {
    Serial.printf("[HTTP} Unable to connect\n");
  }
}

String getComponentRoomIDbyID(String componentID){
  String returnValue = "";
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/components/" + componentID +"/getRoom?smartHomeEntityId="+ homeID)) {


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
        StaticJsonDocument<200> data;
        deserializeJson(data, payload);
        returnValue = data["roomId"].as<String>();
      }
    } else {
      Serial.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }
    http.end();
    return returnValue;
  } else {
    Serial.printf("[HTTP} Unable to connect\n");
    return returnValue;
  }
}

String getComponentType(String componentName){
  if( componentName == "lightBulbID"){
    return "LightBulb";
  } else if (componentName == "motionID") {
    return "MotionSensor";
  } else if (componentName == "brightnessID") {
    return "LightSensor";
  } else if (componentName == "humidityID") {
    return "HumiditySensor";
  } else if(componentName == "temperatureID") {
    return "Thermometer";
  } else {
    return "";
  }
}

void registerComponent(String componentName){
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/components/register?ipAddress=" + wifiIP)){
    http.addHeader("Content-Type", "application/json");
    String componentType = getComponentType(componentName);
    int httpCode = http.POST(componentType);

    if (httpCode > 0) {
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTP] POST... code: %d\n", httpCode);

      if (httpCode == 201 || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
        String payload = http.getString();
        StaticJsonDocument<200> data;
        deserializeJson(data, payload);

        if( componentName == "lightBulbID"){
          lightBulbID = data["componentId"].as<String>();
        } else if (componentName == "motionID") {
          motionID = data["componentId"].as<String>();
        } else if (componentName == "brightnessID") {
          brightnessID = data["componentId"].as<String>();
        } else if (componentName == "humidityID") {
          humidityID = data["componentId"].as<String>();
        } else if(componentName == "temperatureID") {
          temperatureID = data["componentId"].as<String>();
        } else {
        }
        homeID = data["smartHomeEntityId"].as<String>();
        Serial.println(payload);
        Serial.printf("Success getting homeID");
      }
    } else {
      Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }
    http.end();
  } else {
    Serial.printf("[HTTP} Unable to connect\n");
  }
}

void registerAllComponents(){
  registerComponent("lightBulbID");
  registerComponent("motionID");
  registerComponent("brightnessID");
  registerComponent("humidityID");
  registerComponent("temperatureID");
  registerComponent("switchID");
}

void checkMotion(){
  if (digitalRead(MOTION_IN) == HIGH){
    Serial.println("RUCH!");
    motion = HIGH;
    delay(50);
  } else {
    motion = LOW;
    delay(50);
  }
}
