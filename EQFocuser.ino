/**
 * The MIT License
 * Copyright (c) 2016 Everett Quebral Everett.Quebral@gmail.com
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:\
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
 
// EQ Simple Focuser
// Arduino code in controlling an absolute focuser
// using a stepper motor

#include <AccelStepper.h>
// steps
#define FULL_ROTATION 8

// motor pins
#define motorPin5 8 // blue
#define motorPin6 9 // pink
#define motorPin7 10 // yellow
#define motorPin8 11 // orange

// Declaration needed for the AccelStepper Library
AccelStepper stepper1(FULL_ROTATION, motorPin5, motorPin7, motorPin6, motorPin8);

// for command purposes
String inputString = "";
String lastCommand = "";
boolean stringComplete = false;
boolean commandReady = false;
int step = 0;
String com;

void setup() {
  stepper1.setMaxSpeed(1000.0);
  stepper1.setAcceleration(100.0);
  stepper1.setSpeed(10);
  
  Serial.begin(9600);
  inputString.reserve(200);
}

void loop() {

  if (stringComplete){
    stringComplete = false;
    Serial.println(inputString + "#");
    lastCommand = inputString;
    inputString = "";
  }
  
  // we take actions only when commandReady == true
  // and command can only be ready when the stepper has distanceToGo() == 0
  if (lastCommand.length() > 1){
//    Serial.println("stop");
    stepper1.stop();
    commandReady = true;
    if (commandReady){
      commandReady = false;
      Serial.println(lastCommand);

      // COMMANDS that are available will be processed here
      // A - FAST-REVERSE - A 1000 - goto currentPosition() - 1000
      // B - REVERSE
      // C - FORWARD
      // D - FAST-FORWARD
      // E - POSITION - absolute
      // F - GETPOSITION 
      // X - GETREADYSTATUS || 0 = READY, NONZERO = BUSY
      // Z - IDENTIFY || "EQFOCUSER"
      // COMMAND SYNTAX E 1000 - goto absolute position 1000

      com = lastCommand.substring(0,1);
      step = lastCommand.substring(2).toInt();
      // move commmands
      if (com.equals("A") || com.equals("B")){
        step = stepper1.currentPosition() - step;
      }
      if (com.equals("C") || com.equals("D")){
        step = stepper1.currentPosition() + step;
      }
      if (com.equals("E")){
        step = step;
      }

      if (com.equals("A") || com.equals("B") || com.equals("C") || com.equals("D") || com.equals("E")){
        Serial.print("Moving to");
        Serial.print(step);
        Serial.println("#");
        stepper1.runToNewPosition(step);
      }

      if (com.equals("F")){
        Serial.print(stepper1.currentPosition());
        Serial.println("#");
      }

      if (com.equals("Z")){
        Serial.println("EQFOCUSER#");
      }
      

      lastCommand = "";
      // this will update the driver
      Serial.print("POSITION:");
      Serial.print(stepper1.currentPosition());
      Serial.println("#");
    }
  }

  if (stepper1.targetPosition() == stepper1.currentPosition() && lastCommand.length() > 1){
    Serial.println("********************"); 
  }
//  else {
//    // send a message that it's ready
//    Serial.println("FOCUSER READY");
//    Serial.println(stepper1.currentPosition());
//  }
  delay(1000);
}

void serialEvent() {
//  Serial.println("SerialEvent");
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    // add it to the inputString:
    inputString += inChar;
    // if the incoming character is a newline, set a flag
    // so the main loop can do something about it:
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}