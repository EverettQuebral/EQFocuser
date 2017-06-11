# EQ Focuser

An Open Source Computer Controlled Telescope Focuser using Arduino.

## Description

An Absolute ASCOM Driver for EQ Focuser.  The Focuser can be controlled manually or by using ASCOM that Astronomoical Applications use for observation or taking astrophotography.
The EQ Focuser features a very simple interface and manual controller.  It doesn't need to be attached to a computer for manual operation.  For ASCOM Driver operation, it needs to be connected to a USB port.
[![Focuser in Action](http://www.youtube.com/watch?v=E4LpXYvdvyA/0.jpg)](https://www.youtube.com/watch?v=E4LpXYvdvyA "Focuser in Action")

### Supported Stepper Motor Controller

- ULN2003 Motor Driver
- Adafruit Motor/Stepper/Servo Shield for Arduino v2 Kit - v2.3 https://www.adafruit.com/product/1438
- Easy Driver http://www.schmalzhaus.com/EasyDriver/

#### Travis

<img src="https://travis-ci.org/EverettQuebral/EQFocuser.png"/> https://travis-ci.org/EverettQuebral/EQFocuser
Unfortunately, Travis can only test and build on a Mono in a Linux Environment, the test is failing as of this moment


#### Hardware Parts

- 1 Arduino
- 2 Pull Up Switch
- 1 Variable Resistor
- 1 DHT11 (Temperature/Humidity) Sensor
- 1 ULN2003 Motor Driver
- 1 ST35 12v Stepper Motor

##### Diagram

![Component Diagram](Resources/Diagram.png?raw=true "Component Diagram")
![Schematic Diagram](Resources/Schematic.png?raw=true "Schematic Diagram")
![Focuser 2 Motor](Resources/Focuser%202.png?raw=true "2 Motor Diagram")

##### Video

https://www.youtube.com/watch?v=E4LpXYvdvyA&t=5s

##### Screen Shot

###### Setup Dialog

![Setup Dialog](Resources/Version%201.0.3%20Setup%20Dialog.png?raw=true "Setup Dialog")

###### Main Window controller

![Main Window](Resources/Version%201.0.3%20Main%20Form.png?raw=true "Main Window")

###### Advanced Windows controller

![Advanced Window Controller](Resources/Version%201.0.3%20Complete%20Form.png?raw=true "Advanced Window")

###### Driver Download

You can download the latest Driver at the Release section https://github.com/EverettQuebral/EQFocuser/releases

###### 3D Model Files

Available under the Resources folder

![SCT Focuser Bracket](Resources/sct-bracket-3.stl?raw=true "SCT 3D Bracket")
