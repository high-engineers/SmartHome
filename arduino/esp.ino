#include <dht11.h>
#include <ArduinoJson.h>
#include <Arduino.h>

#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClient.h>
#include <WiFiManager.h>
#include <EEPROM.h>

#define MOTION_IN 12  //d6
#define DHT11PIN 13   //d7
#define RELAY_OUT 15  //d8
#define BUZZER_OUT 4  //d2
#define BPIN A0

dht11 DHT11;
int bvalue;
bool initMeasurements = true;
long timer = 0;
long measurementPeriod = 5400000;
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
String homeID = "ec2c6a09-3772-48aa-6c5b-08d7734b257c";
String lightBulbID = "5522a0df-18c4-4a12-05ce-08d77dca541c";
String motionID = "0f30fdcd-3baa-442b-05cf-08d77dca541c";
String brightnessID = "dc074abc-482c-4d40-05d0-08d77dca541c";
String humidityID = "8aba1083-4ce0-4a8d-05d1-08d77dca541c";
String temperatureID = "bbf5b0a0-7b84-4a60-05d2-08d77dca541c";
String alarmID = "ee46359b-5150-4ee5-05d3-08d77dca541c";
//ids locations
int homeIDlocation = 0;
int lightBulbIDlocation = homeIDlocation + sizeof(homeID);
int motionIDlocation = lightBulbIDlocation + sizeof(lightBulbID);
int brightnessIDlocation = motionIDlocation + sizeof(motionID);
int humidityIDlocation = brightnessIDlocation + sizeof(brightnessID);
int temperatureIDlocation = humidityIDlocation + sizeof(humidityID);
int alarmIDlocation = temperatureIDlocation + sizeof(temperatureID);
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
  String State;
};

ComponentState componentsState[20];

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
  Serial.println(uniqueRoomsIDs[0]);
  Serial.println(uniqueRoomsIDs[1]);
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
  minBrightness = 0.0;
  WiFiManager wifiManager;
  //wifiManager.resetSettings();
  Serial.println("Connecting ...");
  wifiManager.autoConnect("SmartHome");
  Serial.println("Connected!");
//  getPublicIP();
//  alarm = true;
  alarmState = 1;
  motionBulb = false;
  motion = LOW;
//  doIhaveAllIDs();
//  printallIDs();
}

void printallIDs(){
  Serial.println("homeID: " + homeID + " * lightBulbID: " + lightBulbID + " * motionID: " + motionID + " brightnessID: " + brightnessID);
  Serial.println("humidityID: " + humidityID + " * temperatureID: " + temperatureID + " * alarmID: " + alarmID);
}

void getPublicIP(){
  http.begin("http://api.ipify.org/");
  int httpCode = http.GET();
  if (httpCode != 200) {
    Serial.println("Unable to get public IP");
  } else {
    wifiIP = http.getString();
    Serial.println("Public IP: " + wifiIP);
  }
}

//void writeGUID(String guid, int addrToWrite){
//  EEPROM.put(addrToWrite, homeID);
//  Serial.println("homeID saved");
//  addrToWrite = addrToWrite + sizeof(homeID);
//  EEPROM.put(addrToWrite, lightBulbID);
//  Serial.println("lightBulbID saved");
//  homeID = "";
//  lightBulbID = "";
//  EEPROM.get(addrToWrite, homeID);
//}

bool checkIfGUID(String guid) {
  for(int i = 8; i < 24 ; i+=5){
    if ( guid.charAt(i) != '-') {
      return false;
    }
  }
  return true;
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
//  if (minBrightness > brightness || (motion == HIGH && motionBulb == true)) {
//    digitalWrite(RELAY_OUT, HIGH);
//    lightBulbState = 1;
//    Serial.println("ZAPALAM ŚWIATŁO");
//    delay(50);
//  } else {
 //     digitalWrite(RELAY_OUT, LOW);
  //    Serial.println("GASZĘ ŚWIATŁO");
  //    delay(50);
   // }
   digitalWrite(RELAY_OUT, lightBulbState);
}

void checkMotion();
void doIhaveAllIDs();
void registerAllComponents();
void registerComponent(String componentName);
String getComponentType(String componentName);
String getComponentRoomIDbyID(String componentID);
void getComponentsStates(String myRoomID);
void writeStates(ComponentState componentsState[]);
void postComponentReadings(String roomID, String data);
void postComponentsReadingsAllRooms();
void postReadingsWhenTime();
void getComponentsStatesAllRooms();
String getComponentRoomIDbyName(String componentName);
void getAllComponentsRoomsIDs();
void setUserVariables(String data);

void loop() {
////  delay(5000);
////  Serial.println("homeID: " + homeID + " * lightBulbID: " + lightBulbID + " * motionID: " + motionID + " brightnessID: " + brightnessID);
////  Serial.println("humidityID: " + humidityID + " * temperatureID: " + temperatureID + " * alarmID: " + alarmID);
////  registerAllComponents();
////  Serial.println("homeID: " + homeID + " * lightBulbID: " + lightBulbID + " * motionID: " + motionID + " brightnessID: " + brightnessID);
////  Serial.println("humidityID: " + humidityID + " * temperatureID: " + temperatureID + " * alarmID: " + alarmID);
//  delay(5000);
//  Serial.println("lightBulbRoomID: " + lightBulbRoomID + " * motionRoomID: " + motionRoomID + " brightnessRoomID: " + brightnessRoomID);
//  Serial.println("humidityRoomID: " + humidityRoomID + " * temperatureRoomID: " + temperatureRoomID + " * alarmRoomID: " + alarmRoomID);
//  getAllComponentsRoomsIDs();
//  Serial.println("lightBulbRoomID: " + lightBulbRoomID + " * motionRoomID: " + motionRoomID + " brightnessRoomID: " + brightnessRoomID);
//  Serial.println("humidityRoomID: " + humidityRoomID + " * temperatureRoomID: " + temperatureRoomID + " * alarmRoomID: " + alarmRoomID);
//  delay(5000);
//  Serial.println("lightBulbState: " + (String)lightBulbState + " * alarmState: " + alarmState);
//  getComponentsStatesAllRooms();
//  delay(1000);
//  Serial.println("lightBulbState: " + (String)lightBulbState + " * alarmState: " + alarmState);
//  postComponentsReadingsAllRooms();
//  delay(5000);
//  delay(10000);
  delay(1000);
  doIhaveAllIDs();
  delay(1000);
  getAllComponentsRoomsIDs();
  getComponentsStatesAllRooms();
  alarmActivation();
  bulbActivation();
  postReadingsWhenTime();
// //getInfoFromServer();
////  if (millis() - postTime > postDelay){
////    postTime += postDelay;
////    postDataToServer();
////  }
  checkMotion();
  delay(1000);
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
  allRoomsIDs[0] = motionRoomID;
  brightnessRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("brightness"));
  allRoomsIDs[1] = brightnessRoomID;
  humidityRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("humidity"));
  allRoomsIDs[2] = humidityRoomID;
  temperatureRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("temperature"));
  allRoomsIDs[3] = temperatureRoomID;
  lightBulbRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("lightBulb"));
  allRoomsIDs[4] = lightBulbRoomID;
  alarmRoomID = getComponentRoomIDbyID(getComponentRoomIDbyName("alarm"));
  allRoomsIDs[5] = alarmRoomID;
  Serial.println(allRoomsIDs[4]);
  getUniqueRoomsIDs(allRoomsIDs);
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
  for (String uniqueRoom : uniqueRoomsIDs){
    if ( uniqueRoomsIDs ){
      Serial.println("Getting states for room:" +  uniqueRoom);
      getComponentsStates(uniqueRoom);
    }
  }
}

void postReadingsWhenTime(){
  if ( initMeasurements == true) {
    timer = millis();
    postComponentsReadingsAllRooms();
    initMeasurements = false;
  } else {
    if (millis() - timer >= measurementPeriod) {
      timer = millis();
      postComponentsReadingsAllRooms();
    }
  }
}

void postComponentsReadingsAllRooms(){
  getMeasurements();
  String data;
  for (String uniqueRoom : uniqueRoomsIDs){
    if ( uniqueRoomsIDs ){
      measurement.clear();
      measurement["SensorId"] = motionID;
      measurement["Reading"] = motion;
      serializeJsonPretty(measurement, data);
      postComponentReadings(uniqueRoom, data);
      measurement.clear();
      data = "";
      measurement["SensorId"] = brightnessID;
      measurement["Reading"] = brightness;
      serializeJsonPretty(measurement, data);
      Serial.println("AUUUUU " + data);
      postComponentReadings(uniqueRoom, data);
      measurement.clear();
      data = "";
      measurement["SensorId"] = humidityID;
      measurement["Reading"] = humidity;
      serializeJsonPretty(measurement, data);
      postComponentReadings(uniqueRoom, data);
      measurement.clear();
      data = "";
      measurement["SensorId"] = temperatureID;
      measurement["Reading"] = temperature;
      serializeJsonPretty(measurement, data);
      postComponentReadings(uniqueRoom, data);
      measurement.clear();
      data = "";
      measurement["SensorId"] = lightBulbID;
      measurement["Reading"] = lightBulbState;
      serializeJsonPretty(measurement, data);
      postComponentReadings(uniqueRoom, data);
    }
  }
}

void postComponentReadings(String roomID, String data){
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/components/collect?smartHomeEntityId=" + homeID + "&roomId=" + roomID)){
    http.addHeader("Content-Type", "application/json");
    Serial.println(data);
    int httpCode = http.POST(data);

    if (httpCode > 0) {
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTP] POST... code: %d\n", httpCode);

      if (httpCode == 200 || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
        String payload = http.getString();
        Serial.println(payload);
        Serial.println("SUCCESS!");
      }
    } else {
      Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
    }
    http.end();
  } else {
    Serial.printf("[HTTP} Unable to connect\n");
  }
}

void writeStates(ComponentState componentsState[]){

  for (int i=0; i < 20; i++){
    Serial.println("LB_ID = " + lightBulbID);
    Serial.println("COMP_ID = " + componentsState[i].Id);
    if(componentsState[i].Id == lightBulbID){
      if ( componentsState[i].State == "On" ) {
        lightBulbState = HIGH;
        Serial.println("LIGHT STATE HIGH!");
      } else if ( componentsState[i].State == "Off" ) {
        lightBulbState = LOW;
        Serial.println("LIGHT STATE LOW!");
      }
    }
    if(componentsState[i].Id == alarmID){
      if ( componentsState[i].State == "On" ) {
        alarmState = HIGH;
        Serial.println("ALARM STATE HIGH!");
      } else if ( componentsState[i].State == "Off" ) {
        alarmState = LOW;
        Serial.println("ALARM STATE LOW!");
      }
    }
  }
}

void getComponentsStates(String myRoomID){
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/components?smartHomeEntityId=" + homeID + "&roomId=" + myRoomID)) {


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
        DynamicJsonDocument data(1024);
        deserializeJson(data, payload);
        JsonArray root = data.as<JsonArray>();
        int i = 0;
        for (JsonVariant item : root) {
          componentsState[i].Id = item["id"].as<String>();
          Serial.println("R: ID: " + componentsState[i].Id);
          componentsState[i].State = item["state"].as<String>();
          i++;
        }
        writeStates(componentsState);
      } else {
        Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
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
  Serial.println("Getting component room ID... componentID: " + componentID + " homeID: " + homeID);
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
        Serial.println("Payload: " + payload);
        StaticJsonDocument<200> data;
        deserializeJson(data, payload);
        
        returnValue = data["roomId"].as<String>();
      } else {
        Serial.printf("[HTTP] GET... failed, error: %s\n", http.errorToString(httpCode).c_str());
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
  } else if(componentName == "alarmID") {
    return "Alarm";
  } else {
    return "";
  }
}

void registerComponent(String componentName){
  Serial.print("[HTTP] begin...\n");
  if (http.begin(client, "http://smarthomehighengineers.azurewebsites.net/api/components/register?smartHomeEntityId=" + homeID)){
    http.addHeader("Content-Type", "application/json");
    String componentType = getComponentType(componentName);
    String postdata;
    StaticJsonDocument<200> json;
    json["Type"] = componentType;
    serializeJsonPretty(json, postdata);
    Serial.print(postdata);
    Serial.println();
    int httpCode = http.POST(postdata);

    if (httpCode > 0) {
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTP] POST... code: %d\n", httpCode);

      if (httpCode == 200 || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
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
        } else if(componentName == "alarmID") {
          alarmID = data["componentId"].as<String>();
        } else {
        }
        //homeID = data["smartHomeEntityId"].as<String>();
        Serial.println(payload);
        Serial.printf("Success getting homeID");
      } else {
        Serial.printf("[HTTP] POST... failed, error: %s\n", http.errorToString(httpCode).c_str());
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
  registerComponent("alarmID");
}

void doIhaveAllIDs(){
  //EEPROM.get(homeIDlocation, pom);
  Serial.println(homeID);
  if (!checkIfGUID(homeID)){
    Serial.println(checkIfGUID(homeID));
   // EEPROM.put(homeIDlocation, homeID);
  }
  Serial.println("home at " + String(homeIDlocation));
  
  //EEPROM.get(lightBulbIDlocation, pom);
  Serial.println(lightBulbID);
  if (!checkIfGUID(lightBulbID)){
    Serial.println(checkIfGUID(lightBulbID));
    registerComponent("lightBulbID");
  //  EEPROM.put(lightBulbIDlocation, lightBulbID); 
  }
    Serial.println("light at " + String(lightBulbIDlocation));
  
 // EEPROM.get(motionIDlocation, pom);
  Serial.println(motionID);
  if (!checkIfGUID(motionID)){
    Serial.println(checkIfGUID(motionID)); 
    registerComponent("motionID");
 //   EEPROM.put(motionIDlocation, motionID); 
  }
    Serial.println("motion at " + String(motionIDlocation));
  
//  EEPROM.get(brightnessIDlocation, pom);
  Serial.println(brightnessID);
  if (!checkIfGUID(brightnessID)){
    Serial.println(checkIfGUID(brightnessID));
    registerComponent("brightnessID");
   // EEPROM.put(brightnessIDlocation, brightnessID); 
  }
    Serial.println("brightness at " + String(brightnessIDlocation));
    
 // EEPROM.get(humidityIDlocation, pom);
  Serial.println(humidityID);
  if (!checkIfGUID(humidityID)){
    Serial.println(checkIfGUID(humidityID));
    registerComponent("humidityID");
   // EEPROM.put(humidityIDlocation, humidityID); 
  }
    Serial.println("humidity at " + String(humidityIDlocation));
  
  //EEPROM.get(temperatureIDlocation, pom);
  Serial.println(temperatureID);
  if (!checkIfGUID(temperatureID)){
    Serial.println(checkIfGUID(temperatureID));
    registerComponent("temperatureID");
 //   EEPROM.put(temperatureIDlocation, temperatureID); 
  }
    Serial.println("temperature at " + String(temperatureIDlocation));
  
  //EEPROM.get(alarmIDlocation, pom);
  Serial.println(alarmID);
  if (!checkIfGUID(alarmID)){
    Serial.println(checkIfGUID(alarmID));
    registerComponent("alarmID");
 //   EEPROM.put(alarmIDlocation, alarmID);
  }
    Serial.println("alarm at " + String(alarmIDlocation));
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
