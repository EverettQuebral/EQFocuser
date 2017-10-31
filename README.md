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

- 1 Arduino (https://www.amazon.com/Gikfun-ATmega328-Micro-controller-Arduino-Ek1620x1/dp/B00Q9YBO88/ref=sr_1_10?s=electronics&ie=UTF8&qid=1509426881&sr=1-10&keywords=arduino+nano)
- 2 Pull Up Switch (Optional) 
- 1 Variable Resistor (Optional)
- 1 DHT11 (Temperature/Humidity) Sensor (Optional)

##### Nema17 Motor
- 1 EasyDriver or Adafruit Motor Controller (https://www.amazon.com/Gikfun-EasyDriver-Shield-Stepper-Arduino/dp/B00RCTW5SM/ref=sr_1_2_sspa?s=electronics&ie=UTF8&qid=1509426935&sr=1-2-spons&keywords=easydriver+stepper+motor+driver&psc=1)
- 1 NEMA 17 v0.7Amp (https://www.amazon.com/Shaft-Stepper-1-8deg-42x42x34mm-4-wire/dp/B00W96L85Y/ref=sr_1_17?s=electronics&ie=UTF8&qid=1509426959&sr=1-17&keywords=nema+17)

##### ST35 or 28BYJ-48
- 1 ULN2003 Motor Driver (https://www.amazon.com/4-phase-Stepper-28byj-48-Atomic-Market/dp/B01DLDW9S8/ref=sr_1_23?s=electronics&ie=UTF8&qid=1509427016&sr=1-23&keywords=uln2003+driver+board)
- 1 ST35 (https://shop4.frys.com/product/7726708?site=sr:SEARCH:MAIN_RSLT_PG)
or 
- 1 28BYJ-48 (https://www.amazon.com/4-phase-Stepper-28byj-48-Atomic-Market/dp/B01DLDW9S8/ref=sr_1_23?s=electronics&ie=UTF8&qid=1509427016&sr=1-23&keywords=uln2003+driver+board)


##### Diagrams

###### Diagram Using ULN2003 Motor Driver
![Component Diagram](Resources/Diagram.png?raw=true "Component Diagram")
![Schematic Diagram](Resources/Schematic.png?raw=true "Schematic Diagram")
![Focuser 2 Motor](Resources/Focuser%202.png?raw=true "2 Motor Diagram")

###### Diagram Using EasyDriver
![Component Diagram Easy Driver](Resources/Example1_6_bb.png?raw=true "Component Diagram using Easy Driver")


![Easy Driver](Resources/EasyDriver_V44_Description.png?raw=true "Easy Driver Pin Layout")

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
