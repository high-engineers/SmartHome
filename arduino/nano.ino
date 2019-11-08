
#define MOTION_IN 5
#define MOTION_OUT 10

long timer = 0;
long motionTime = 4000;


void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  pinMode(MOTION_IN, INPUT);
  pinMode(MOTION_OUT, OUTPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
  if(digitalRead(MOTION_IN) == HIGH){
    Serial.println("WYKRYŁEM RUCH!");
    digitalWrite(MOTION_OUT, HIGH);
   // timer = millis();
    delay(200);
  } else {
    digitalWrite(MOTION_OUT, LOW);
    delay(200);
  }

  if(digitalRead(MOTION_OUT) == HIGH){
    Serial.print("Przesyłam HIGH");
  } else {
    Serial.println("Przesyłam LOW");
  }
  delay(1000);
//  if ( millis() - timer > motionTime){
//    digitalWrite(MOTION_OUT, LOW);
//  } else {
//    Serial.println(":O");
//    delay(500);
//  }

}
