// EQ Simple Focuser
// Arduino code in controlling an absolute focuser
// using a stepper motor

#include <AccelStepper.h>
#include <dht.h>

// motor pins
#define motorPin5 8 // blue
#define motorPin6 9 // pink
#define motorPin7 10 // yellow
#define motorPin8 11 // orange

// for the temperature and hubmidity sensor
#define DHT11_PIN 5

// Declaration needed for the AccelStepper Library
AccelStepper stepper1(AccelStepper::FULL4WIRE, motorPin5, motorPin7, motorPin6, motorPin8);

// for command purposes
String inputString = "";
int step = 0;

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

// temperature and humidity sensor
dht DHT;
int chkSensor;

void setup() {
	Serial.begin(115200);
	Serial.println("EQFOCUSER#");

	stepper1.setMaxSpeed(100.0);
	stepper1.setAcceleration(100.0);
	stepper1.setSpeed(100);

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
			stepper1.moveTo(toPosition);
		}
		if (cwVal == LOW) {
			toPosition = stepper1.currentPosition() + variableResistorValue / 10;
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
	case 'a': _newPosition = _currentPosition - (_step * 2);
		break;
	case 'B':  // REVERSE "<"
	case 'b': _newPosition = _currentPosition - _step;
		break;
	case 'C':  // FORWARD ">"
	case 'c': _newPosition = _currentPosition + _step;
		break;
	case 'D':  // FAST FORWARD ">>"
	case 'd': _newPosition = _currentPosition + (_step * 2);
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
		humidityTemperatureReport();
		break;
	case 'X':  // GET STATUS - may not be needed
	case 'x':
		stepper1.stop();
		break;
	case 'Z':  // IDENTIFY
	case 'z':  _answer += "EQFOCUSER";
		break;
	default:
		_answer += "EQFOCUSER";
		break;
	}

	if (_newPosition != _currentPosition) {
		// a move command was issued
		Serial.print("MOVING:");
		Serial.print(_newPosition);
		Serial.println("#");
		//    stepper1.runToNewPosition(_newPosition);  // this will block the execution
		stepper1.moveTo(_newPosition);
		stepper1.runSpeedToPosition();
		_answer += "POSITION:";
		_answer += _newPosition;
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
