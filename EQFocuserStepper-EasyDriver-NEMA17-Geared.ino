// EQ Simple Focuser
// Arduino code in controlling an absolute focuser
// using a stepper motor

#include <AccelStepper.h>

// Define the stepper and the pins it will use
AccelStepper stepper1(AccelStepper::DRIVER, 9, 8);

// for command purposes
String inputString = "";
int step = 0;
int backlashStep = 0;
String lastDirection = "NONE"; //"OUTWARD"
String currentDirection = "NONE";

// for pin values
int ccwPin = 7;
int cwPin = 6;
int ccwVal = 0;
int cwVal = 0;

// for the variable resistor
int potpin = 0;
int variableResistorValue = 10;
int oldVariableResistorValue = 10;

// for manual control
long toPosition;
bool positionReported = false;


void setup() {
  Serial.begin(115200);
  Serial.println("EQFOCUSER_STEPPER#");
  
  stepper1.setMaxSpeed(10000.0);
  stepper1.setAcceleration(3000.0);
//  stepper1.setSpeed(3000);

  inputString.reserve(200);

  pinMode(ccwPin, INPUT_PULLUP);
  pinMode(cwPin, INPUT_PULLUP);
}


void loop() {
  variableResistorValue = analogRead(potpin);

  cwVal = digitalRead(cwPin);
  ccwVal = digitalRead(ccwPin);

  if (ccwVal == LOW || cwVal == LOW) {
    //    if (oldVariableResistorValue < variableResistorValue - 5 || oldVariableResistorValue > variableResistorValue + 5 ){
    //      // set the max acceleration
    //      stepper1.setAcceleration(variableResistorValue / 102.4);
    //      oldVariableResistorValue = variableResistorValue;
    //      Serial.print("Changing Value of acceleration");
    //      Serial.println(oldVariableResistorValue);
    //    }
    // the PULLUP Pins are pressed
    Serial.print("MOVING:");
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
      reportPosition();
    }
  }
  else {
    if (stepper1.distanceToGo() != 0) {
      // let the stepper finish the movement
      stepper1.run();
      positionReported = false;
    }
    if (stepper1.distanceToGo() == 0 && !positionReported) {
      reportPosition();
      delay(500);
      positionReported = true;
    }
  }
}

void reportPosition() {
  Serial.print("POSITION:");
  Serial.print(stepper1.currentPosition());
  Serial.println("#");
}

// test if direction is the same, otherwise apply backlash step
// this method is only applicable for manual focusing changes
void applyBacklashStep(int toPosition, String lastDirection, String currentDirection){
  if (lastDirection == currentDirection){
    // no backlash
    stepper1.moveTo(toPosition);
  }
  else {
    // apply backlash
    stepper1.moveTo(toPosition + backlashStep);
    stepper1.setCurrentPosition(toPosition - backlashStep);
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
  int _currentPosition = stepper1.currentPosition();
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
    stepper1.setCurrentPosition(0);
    break;
  case 'H':  // SET ACCELERATION
  case 'h': _newPosition = _currentPosition; // non move command
    stepper1.setAcceleration(_step);
    _answer += "SET-ACCELERATION:";
    _answer += _step;
    break;
  case 'I':  // SET SPEED
    _newPosition = _currentPosition; // non move command
    stepper1.setSpeed(_step);
    _answer += "SET-SPEED:";
    _answer += _step;
    break;
  case 'i':  // GET SPEED
    _newPosition = _currentPosition; // non move command
    _answer += "GET-SPEED:";
    _answer += stepper1.speed();
    break;
  case 'J':  // SET MAX SPEED
  case 'j':  _newPosition = _currentPosition; // non move command
    stepper1.setMaxSpeed(_step);
    _answer += "SET-MAXSPEED:";
    _answer += _step;
    break;
  case 'k': // GET TEMPERATURE / HUMIDITY
    _newPosition = _currentPosition; // non move command
    break;
  case 'L' :
  case 'l' :
    backlashStep = _step;
    _answer += "SET-BACKLASHSTEP:";
    _answer += _step;
    break;
  case 'X':  // GET STATUS - may not be needed
  case 'x':
    stepper1.stop();
    break;
  case 'Z':  // IDENTIFY
  case 'z':  _answer += "EQFOCUSER_STEPPER";
    break;
  default:
    _answer += "EQFOCUSER_STEPPER";
    break;
  }

  if (_newPosition != _currentPosition) {
    if (lastDirection != "NONE"){
      if (stepper1.currentPosition() < _newPosition){
        // moving forward
        currentDirection == "OUTWARD";
      }
      if (stepper1.currentPosition() > _newPosition){
        // moving backward
        currentDirection == "INWARD";
      }
      Serial.print(lastDirection);Serial.print("===");Serial.println(currentDirection);
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
    stepper1.moveTo(_newPosition);
    stepper1.runSpeedToPosition();
    lastDirection = currentDirection;
    _answer += "POSITION:";
    _answer += stepper1.currentPosition();
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
