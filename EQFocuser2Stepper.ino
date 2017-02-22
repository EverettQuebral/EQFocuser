// EQ Simple Focuser
// Arduino code in controlling an absolute focuser
// using a stepper motor

#include <AccelStepper.h>
//#include <MultiStepper.h>
#include <dht.h>

// first motor pins
#define motor1Pin5 12 // blue
#define motor1Pin6 11 // pink
#define motor1Pin7 10 // yellow
#define motor1Pin8 9 // orange

// second motor pins
#define motor2Pin5 8
#define motor2Pin6 7
#define motor2Pin7 6
#define motor2Pin8 5

// for the temperature and hubmidity sensor
#define DHT11_PIN 13

// Declaration needed for the AccelStepper Library
AccelStepper stepper1(AccelStepper::FULL4WIRE, motor1Pin5, motor1Pin7, motor1Pin6, motor1Pin8);
AccelStepper stepper2(AccelStepper::FULL4WIRE, motor2Pin5, motor2Pin7, motor2Pin6, motor2Pin8);

// for current stepper action
int currentStepper = 0; // 0 - first stepper, 1 - second stepper, 0 = default
// for command purposes
String inputString = "";
int step = 0;
int backlashStep = 0;
String lastDirection = "NONE"; //"OUTWARD"
String currentDirection = "NONE";

// for pin values
int ccwPin = 4;
int cwPin = 3;
int ccwVal = 0;
int cwVal = 0;
int stepperSelect = 2;
int stepperSelectValue = 0;

// for the variable resistor
int potpin = 0;
int variableResistorValue = 10;
int oldVariableResistorValue = 10;

// for manual control
long toPosition;
bool positionReported = false;

// temperature and humidity sensor
dht DHT;
int chkSensor;

void setup() {
  Serial.begin(115200);
  Serial.println("EQFOCUSER_STEPPER2#");
  
  stepper1.setMaxSpeed(100.0);
  stepper1.setAcceleration(100.0);
  stepper1.setSpeed(100);

  stepper2.setMaxSpeed(100.0);
  stepper2.setAcceleration(100.0);
  stepper2.setSpeed(100);

  inputString.reserve(200);

  pinMode(ccwPin, INPUT_PULLUP);
  pinMode(cwPin, INPUT_PULLUP);
  pinMode(stepperSelect, INPUT_PULLUP);
}


void loop() {
  variableResistorValue = analogRead(potpin);

  cwVal = digitalRead(cwPin);
  ccwVal = digitalRead(ccwPin);
  stepperSelectValue = digitalRead(stepperSelect);

  // should read the currentStepper here
  if (stepperSelectValue == LOW){
//    Serial.print("CURRENT MOTOR : ");
//    Serial.println(currentStepper);
//    Serial.println("SELECTING MOTOR");
    if (currentStepper == 0) {
      currentStepper = 1;
    }
    else if (currentStepper == 1) {
      currentStepper = 0;
    }
    delay(300);
    Serial.print("SELECTEDMOTOR:");
    Serial.print(currentStepper);
    Serial.println("#");
  }

  if (currentStepper == 0){
    moveStepper1(variableResistorValue);
  }
  else if (currentStepper == 1){
    moveStepper2(variableResistorValue);
  }

}

void moveStepper1(int variableResistorValue){
   if (ccwVal == LOW || cwVal == LOW) {
    //    if (oldVariableResistorValue < variableResistorValue - 5 || oldVariableResistorValue > variableResistorValue + 5 ){
    //      // set the max acceleration
    //      stepper1.setAcceleration(variableResistorValue / 102.4);
    //      oldVariableResistorValue = variableResistorValue;
    //      Serial.print("Changing Value of acceleration");
    //      Serial.println(oldVariableResistorValue);
    //    }
    // the PULLUP Pins are pressed
    Serial.print("MOVING STEPPER1:");
    if (ccwVal == LOW) {
      toPosition = stepper1.currentPosition() - variableResistorValue / 10;
      Serial.print(toPosition);
      applyBacklashStep(toPosition, lastDirection, "INWARD");
//      stepper1.moveTo(toPosition);
      lastDirection = "INWARD";
      
    }
    if (cwVal == LOW) {
      toPosition = stepper1.currentPosition() + variableResistorValue / 10;
      Serial.print(toPosition);
      applyBacklashStep(toPosition, lastDirection, "OUTWARD");
//      stepper1.moveTo(toPosition);
      lastDirection = "OUTWARD";
    }
    Serial.println("#");
    stepper1.run();

    if (toPosition == stepper1.currentPosition()) {
      // the stepper is not really moving here so just report the posiiton
      reportPosition(stepper1.currentPosition());
    }
  }
  else {
    if (stepper1.distanceToGo() != 0) {
      // let the stepper finish the movement
      stepper1.run();
      positionReported = false;
    }
    if (stepper1.distanceToGo() == 0 && !positionReported) {
      reportPosition(stepper1.currentPosition());
      delay(500);
      positionReported = true;
    }
  }
}

void moveStepper2(int variableResistorValue){
   if (ccwVal == LOW || cwVal == LOW) {
    Serial.print("MOVING STEPPER2:");
    if (ccwVal == LOW) {
      toPosition = stepper2.currentPosition() - variableResistorValue / 10;
      Serial.print(toPosition);
      applyBacklashStep(toPosition, lastDirection, "INWARD");
      lastDirection = "INWARD";
      
    }
    if (cwVal == LOW) {
      toPosition = stepper2.currentPosition() + variableResistorValue / 10;
      Serial.print(toPosition);
      applyBacklashStep(toPosition, lastDirection, "OUTWARD");
      lastDirection = "OUTWARD";
    }
    Serial.println("#");
    stepper2.run();

    if (toPosition == stepper2.currentPosition()) {
      reportPosition(stepper2.currentPosition());
    }
  }
  else {
    if (stepper2.distanceToGo() != 0) {
      stepper2.run();
      positionReported = false;
    }
    if (stepper2.distanceToGo() == 0 && !positionReported) {
      reportPosition(stepper2.currentPosition());
      delay(500);
      positionReported = true;
    }
  }
}
void reportPosition(int currentPosition) {
  Serial.print("POSITION:");
  Serial.print(currentPosition);
  Serial.println("#");
}

// test if direction is the same, otherwise apply backlash step
// this method is only applicable for manual focusing changes
void applyBacklashStep(int toPosition, String lastDirection, String currentDirection){
  if (lastDirection == currentDirection){
    // no backlash
    if (currentStepper == 0){
      stepper1.moveTo(toPosition);
    }
    else if (currentStepper == 1){
      stepper2.moveTo(toPosition);
    }
  }
  else {
    // apply backlash
    if (currentStepper == 0){
      stepper1.moveTo(toPosition + backlashStep);
      stepper1.setCurrentPosition(toPosition - backlashStep);
    }
    else if (currentStepper == 1){
      stepper2.moveTo(toPosition + backlashStep);
      stepper2.setCurrentPosition(toPosition - backlashStep);
    }
  }
}

/**
* process the command we recieved from the client
* command format is <Letter><Space><Integer>
* i.e. A 500 ---- Fast Rewind with 500 steps
*/
void serialCommand(String commandString) {
  char _command = commandString.charAt(0);
  int _step = commandString.substring(2).toInt();
  String _answer = "";
  // current position of the current stepper
  int _currentPosition = currentStepper == 0 ? stepper1.currentPosition() : stepper2.currentPosition();
  int _newPosition = _currentPosition;
  int _backlashStep;


  
  switch (_command) {
  case 'A':  // FAST REVERSE "<<"
  case 'a': _newPosition = _currentPosition - ( _step * 2 );
            currentDirection = "INWARD";
    break;
  case 'B':  // REVERSE "<"
  case 'b': _newPosition = _currentPosition - _step;
            currentDirection = "INWARD";
    break;
  case 'C':  // FORWARD ">"
  case 'c': _newPosition = _currentPosition + _step;
            currentDirection = "OUTWARD";
    break;
  case 'D':  // FAST FORWARD ">>"
  case 'd': _newPosition = _currentPosition + ( _step * 2 );
            currentDirection = "OUTWARD";
    break;
  case 'E':  // MOVE TO POSITION
  case 'e': _newPosition = _step;
    break;
  case 'F':  // GET CURRENT POSITION
  case 'f': _answer += _currentPosition;
    break;
  case 'G':  // SET POSITION TO 0
  case 'g': _newPosition = 0;
    _currentPosition = 0;
    if (currentStepper == 0) stepper1.setCurrentPosition(0);
    if (currentStepper == 1) stepper2.setCurrentPosition(0);
    break;
  case 'H':  // SET ACCELERATION
  case 'h': _newPosition = _currentPosition; // non move command
    if (currentStepper == 0) stepper1.setAcceleration(_step);
    if (currentStepper == 1) stepper2.setAcceleration(_step);
    _answer += "SET-ACCELERATION:";
    _answer += _step;
    break;
  case 'I':  // SET SPEED
    _newPosition = _currentPosition; // non move command
    if (currentStepper == 0) stepper1.setSpeed(_step);
    if (currentStepper == 1) stepper2.setSpeed(_step);
    _answer += "SET-SPEED:";
    _answer += _step;
    break;
  case 'i':  // GET SPEED
    _newPosition = _currentPosition; // non move command
    _answer += "GET-SPEED:";
    if (currentStepper == 0) _answer += stepper1.speed();
    if (currentStepper == 1) _answer += stepper2.speed();
    break;
  case 'J':  // SET MAX SPEED
  case 'j':  _newPosition = _currentPosition; // non move command
    if (currentStepper == 0) stepper1.setMaxSpeed(_step);
    if (currentStepper == 1) stepper2.setMaxSpeed(_step);
    _answer += "SET-MAXSPEED:";
    _answer += _step;
    break;
  case 'k': // GET TEMPERATURE / HUMIDITY
    _newPosition = _currentPosition; // non move command
    humidityTemperatureReport();
    break;
  case 'L' :
  case 'l' :
    backlashStep = _step;
    _answer += "SET-BACKLASHSTEP:";
    _answer += _step;
    break;
  case 'M' :
  case 'm' :
    currentStepper = _step;
    _answer += "SETTING MOTOR:";
    _answer += _step;
    break;
  case 'n' :
  case 'N' :
    _answer += "GETTING MOTOR:";
    _answer += currentStepper;
    break;
  case 'X':  // GET STATUS - may not be needed
  case 'x':
    // REGARDLESS OF WHICH ONE, DO A STOP
    stepper1.stop();
    stepper2.stop();
    break;
  case 'Z':  // IDENTIFY
  case 'z':  _answer += "EQFOCUSER_STEPPER2";
    break;
  default:
    _answer += "EQFOCUSER_STEPPER2";
    break;
  }

  if (_newPosition != _currentPosition) {
    if (lastDirection != "NONE"){
      if (currentStepper == 0) {
        if (stepper1.currentPosition() < _newPosition){
          // moving forward
          currentDirection == "OUTWARD";
        }
        if (stepper1.currentPosition() > _newPosition){
          // moving backward
          currentDirection == "INWARD";
        }
      }
      if (currentStepper == 1){
        if (stepper2.currentPosition() < _newPosition){
          // moving forward
          currentDirection == "OUTWARD";
        }
        if (stepper2.currentPosition() > _newPosition){
          // moving backward
          currentDirection == "INWARD";
        }
      }
      Serial.print(lastDirection);
      Serial.print("===");
      Serial.println(currentDirection);
      if (lastDirection != currentDirection){
        if (currentDirection == "OUTWARD") _newPosition = _newPosition + backlashStep;
        if (currentDirection == "INWARD") _newPosition = _newPosition - backlashStep;
      }
      else {
        _backlashStep = 0;
      }
    }
  
    // a move command was issued
    Serial.print("MOVING:");
    Serial.print(_newPosition);
    Serial.println("#");
    //    stepper1.runToNewPosition(_newPosition);  // this will block the execution
    if (currentStepper == 0) {
      stepper1.moveTo(_newPosition);
      stepper1.runSpeedToPosition();
    }
    if (currentStepper == 1) {
      stepper2.moveTo(_newPosition);
      stepper2.runSpeedToPosition();
    }
    lastDirection = currentDirection;
    _answer += "POSITION:";
    
    if (currentStepper == 0) _answer += stepper1.currentPosition();
    if (currentStepper == 1) _answer += stepper2.currentPosition();
  }


  Serial.print(_answer);
  Serial.println("#");
}

/**
* handler for the serial communicationes
* calls the SerialCommand whenever a new command is received
*/
void serialEvent() {
  while (Serial.available()) {
    char inChar = (char)Serial.read();
    inputString += inChar;
    if (inChar == '\n') {
      serialCommand(inputString);
      inputString = "";
    }
  }
}

/**
* for DHT routine
*/
void humidityTemperatureReport() {
  chkSensor = DHT.read11(DHT11_PIN);
  switch (chkSensor) {
  case DHTLIB_OK:
    Serial.print("TEMPERATURE:");
    Serial.print(DHT.temperature, 1);
    Serial.println("#");
    delay(50);
    Serial.print("HUMIDITY:");
    Serial.print(DHT.humidity, 1);
    Serial.println("#");
    delay(50);
    break;
  case DHTLIB_ERROR_CHECKSUM:
    Serial.print("TEMPERATURE:");
    Serial.print("CHECKSUMERROR");
    Serial.println("#");
    Serial.print("HUMIDITY:");
    Serial.print("CHECKSUMERROR");
    Serial.println("#");
    break;
  case DHTLIB_ERROR_TIMEOUT:
    Serial.print("TEMPERATURE:");
    Serial.print("TIMEOUTERROR");
    Serial.println("#");
    Serial.print("HUMIDITY:");
    Serial.print("TIMEOUTERROR");
    Serial.println("#");
    break;
  default:
    Serial.print("TEMPERATURE:");
    Serial.print("UNKNOWNERROR");
    Serial.println("#");
    Serial.print("HUMIDITY:");
    Serial.print("UNKNOWNERROR");
    Serial.println("#");
    break;
  }
}
