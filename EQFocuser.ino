// EQ Simple Focuser
// Arduino code in controlling an absolute focuser
// using a stepper motor

#include <AccelStepper.h>

// motor pins
#define motorPin5 8 // blue
#define motorPin6 9 // pink
#define motorPin7 10 // yellow
#define motorPin8 11 // orange

// Declaration needed for the AccelStepper Library
AccelStepper stepper1(AccelStepper::FULL4WIRE, motorPin5, motorPin7, motorPin6, motorPin8);

// for command purposes
String inputString = "";
String lastCommand = "";
boolean stringComplete = false;
boolean commandReady = false;
int step = 0;
String com;

// for pin values
int ccwPin = 7;
int cwPin = 6;
int ccwVal = 0;
int cwVal = 0;

// for the variable resistor
int potpin = 0;
int variableResistorValue = 10;

// for manual control
long toPosition;
bool positionReported = false;

void setup() {
	stepper1.setMaxSpeed(100.0);
	stepper1.setAcceleration(100.0);
	stepper1.setSpeed(100);

	Serial.begin(9600);
	inputString.reserve(200);

	pinMode(ccwPin, INPUT_PULLUP);
	pinMode(cwPin, INPUT_PULLUP);
}


void loop() {
	variableResistorValue = analogRead(potpin);
	cwVal = digitalRead(cwPin);
	ccwVal = digitalRead(ccwPin);

	if (ccwVal == LOW || cwVal == LOW) {
		// the PULLUP Pins are pressed
		Serial.print("MOVING:");
		if (ccwVal == LOW) {
			toPosition = stepper1.currentPosition() - variableResistorValue / 102.4;
			Serial.print(toPosition);
			stepper1.moveTo(toPosition);
		}
		if (cwVal == LOW) {
			toPosition = stepper1.currentPosition() + variableResistorValue / 102.4;
			Serial.print(toPosition);
			stepper1.moveTo(toPosition);
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
			// report the position so the focuser state can change
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

	switch (_command) {
	case 'A':  // FAST REVERSE "<<"
	case 'a': _newPosition = _currentPosition - _step;
		break;
	case 'B':  // REVERSE "<"
	case 'b': _newPosition = _currentPosition - _step;
		break;
	case 'C':  // FORWARD ">"
	case 'c': _newPosition = _currentPosition + _step;
		break;
	case 'D':  // FAST FORWARD ">>"
	case 'd': _newPosition = _currentPosition + _step;
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
	case 'X':  // GET STATUS - may not be needed
	case 'x':
		break;
	case 'Z':  // IDENTIFY
	case 'z':  _answer += "EQFOCUSER";
		break;
	default:
		break;
	}

	if (_newPosition != _currentPosition) {
		// a move command was issued
		Serial.print("MOVING:");
		Serial.print(_newPosition);
		Serial.println("#");
		stepper1.runToNewPosition(_newPosition);  // this will block the execution
		_answer += "POSITION:";
		_answer += _newPosition;
	}

	Serial.print(_answer);
	Serial.println("#");
	// we need some delay here so the answer can be picked up by the client
	//  delay(100);
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
			stringComplete = true;  // deprecated
		}
	}
}
