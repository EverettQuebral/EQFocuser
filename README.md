## EQ Focuser

#### This is an ASCOM Driver for EQ Focuser

[![Focuser in Action](https://www.youtube.com/watch?v=E4LpXYvdvyA)](https://www.youtube.com/watch?v=E4LpXYvdvyA "Focuser in Action")

#### Description

This is an open source project for controlling a stepper motor controlled by Arduino, [more here next time]

### ASCOM Focuser Driver (C#)

##### You have just created the skeleton of an ASCOM Focuser driver in C#. It produces an in-process (assembly) based driver.

* * *

Prior to developing your first driver, please familiarize yourself with the [developer information we've provided](http://ascom-standards.org/developer.html) at the ASCOM Initiative web site (internet required).

You must do the following in order to complete your implementation:

1. Switch to the Debug configuration and build the template now. It should build without errors. 
2. Add a test project to the solution. There are templates that can be used to add either a console or a Windows Forms application: 

- Select the ASCOM Test Forms App (CS) or ASCOM Test Console App (CS) template. 
- Set a name for the test application and click on OK. 
- In the Wizard: set the same device type and model name as for the driver and select Create to build the test project. 
- Set the Test Application to Run at Startup. 
- Click on Debug and the test application should run. You should be able to select your application in the chooser. Selecting Properties should show the default setup dialog for your driver. 
- Trying to continue will generate errors because the additional properties have not been implemented. 

1. Go through the Driver.cs file and replace the System.NotImplemented exceptions with code to implement your driver's functionality. See the ASCOM IFocuserV2 spec. If a property or method is not implemented in your driver the System.NotImplemented exception must be replaced by an ASCOM.PropertyNotImplemented or ASCOM.MethodNotImplemented exception. 
2. Customize the Setup Dialog (SetupDialogForm) to provide the settings and other controls for your driver. You can bind settings directly to controls on your dialog form, there's no need to manage settings manually. A custom Settings class takes care of managing your settings behind the scenes. 

#### Notes:

- Successfully building the driver, as well as using regasm on the assembly, registers it for both COM and ASCOM (the Chooser). See the code in the ASCOM Registration region of Driver.vb. 
- Doing a Clean for the project, as well doing a regasm -u on the assembly, unregisters it for both COM and ASCOM (the Chooser). 
- Place a breakpoint in your driver class constructor, then start debugging (go, F5). Your breakpoint will be hit when the test application creates an instance of your driver (after selecting it in the Chooser). You can now single step, examine variables, etc. Please review the test application and make changes and additions to activate various parts of your driver during debugging. 
- The project's Debug configuration is already configured (The test application creates an instance of your driver (after selecting it in the Chooser). You can now single step, examine variables, etc. Please review the test application and feel free to make changes and additions to activate various parts of your driver during debugging. 

 

 

#### ASCOM Initiative

 

The ASCOM Initiative consists of a group of astronomy software developers and instrument vendors whose goals are to promote the driver/client model and scripting automation.

See the [ASCOM web site](http://ascom-standards.org/) for more information. Please participate in the [ASCOM-Talk Yahoo Group](http://tech.groups.yahoo.com/group/ASCOM-Talk).