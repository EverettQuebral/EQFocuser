## EQ Focuser

An Absolute ASCOM Driver for EQ Focuser.  The Focuser can be controlled manually or by using ASCOM that Astronomoical Applications use for observation or taking astrophotography.
The EQ Focuser features a very simple interface and manual controller.  It doesn't need to be attached to a computer for manual operation.  For ASCOM Driver operation, it needs to be connected to a USB port.

#### Travis
<img src="https://travis-ci.org/EverettQuebral/EQFocuser.png"/> https://travis-ci.org/EverettQuebral/EQFocuser
Unfortunately, Travis can only test and build on a Mono in a Linux Environment, the test is failing as of this moment

#### Description
This is an open source project for controlling a stepper motor controlled by Arduino
[![Focuser in Action](http://www.youtube.com/watch?v=E4LpXYvdvyA/0.jpg)](https://www.youtube.com/watch?v=E4LpXYvdvyA "Focuser in Action")

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

##### Screen Shot

###### Setup Dialog
![Setup Dialog](Resources/SetupScreen.png?raw=true "Setup Dialog")

###### Main Window controller
![Main Window](Resources/MainWindow.png?raw=true "Main Window")
