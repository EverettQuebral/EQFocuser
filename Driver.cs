//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Focuser driver for EQFocuser
//
// Description:	EQFocuser is an ASCOM Driver for running the EQfocuser.ino code for Arduino. 
//
// Implements:	ASCOM Focuser interface version: 6.2.0
// Author:		Everett Quebral <everett.quebral@gmail.com>
//
// Edit Log:
//
// Date			Who	             Vers	Description
// -----------	---------------- -----	-------------------------------------------------------
// 02-oct-2016	Everett Quebral	 6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code canbe deleted and this definition removed.
#define Focuser

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;
using System.IO.Ports;

namespace ASCOM.EQFocuser
{
    //
    // Your driver's DeviceID is ASCOM.EQFocuser.Focuser
    //
    // The Guid attribute sets the CLSID for ASCOM.EQFocuser.Focuser
    // The ClassInterface/None addribute prevents an empty interface called
    // _EQFocuser from being created and used as the [default] interface
    //
    //

    /// <summary>
    /// ASCOM Focuser Driver for EQFocuser.
    /// </summary>
    [Guid("3e7e4b90-27c9-43e2-9dfa-b198333e9cd3")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Focuser : IFocuserV2
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal static string driverID = "ASCOM.EQFocuser.Focuser";
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        private static string driverDescription = "ASCOM Focuser Driver for EQFocuser.";
        internal static string comPortProfileName = "COM Port"; // Constants used for Profile persistence
        internal static string showUIProfileName = "Show Controller";
        internal static string comPortDefault = "COM1";
        internal static string traceStateProfileName = "Trace Level";
        internal static string traceStateDefault = "false";
        internal static string showUIDefault = "true";

        internal static string comPort; // Variables to hold the currrent device configuration
        internal static bool traceState;
        internal static bool showUI;
        

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        private bool connectedState;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Private variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
        /// </summary>
        private TraceLogger tl;

        /// <summary>
        /// Private variable to hold the serial port object
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// Private variable that hold the value whether the focuser is moving or not
        /// </summary>
        private bool isMoving = false;

        private double temperature;

        private double humidity;

        /// <summary>
        /// Private variable that hold the reference to the Main Window
        /// </summary>
        private MainWindow mainWindow;

        public event EventHandler<FocuserValueChangedEventArgs> FocuserValueChanged;

        public event EventHandler<FocuserStateChangedEventArgs> FocuserStateChanged;

        public event EventHandler<FocuserTemperatureChangedEventArgs> FocuserTemperatureChanged;

        public event EventHandler<FocuserHumidityChangedEventArgs> FocuserHumidityChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="EQFocuser"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Focuser()
        {
            ReadProfile(); // Read device configuration from the ASCOM Profile store

            tl = new TraceLogger("", "EQFocuser");
            tl.Enabled = traceState;
            tl.LogMessage("Focuser", "Starting initialisation");

            connectedState = false; // Initialise connected to false
            utilities = new Util(); //Initialise util object
            
            astroUtilities = new AstroUtils(); // Initialise astro utilities object
            tl.LogMessage("Focuser", "Completed initialisation");
        }

        private string message;
        private string existingMessage;

        //
        // PUBLIC COM INTERFACE IFocuserV2 IMPLEMENTATION
        //

        #region Event Handling
        public virtual void OnFocuserValueChanged(FocuserValueChangedEventArgs e)
        {
            if (FocuserValueChanged != null)
            {
                FocuserValueChanged(this, e);
            }
        }

        public virtual void OnFocuserStateChanged(FocuserStateChangedEventArgs e)
        {
            if (FocuserStateChanged != null)
            {
                FocuserStateChanged(this, e);
            }
        }

        public virtual void OnFocuserTemperatureChanged(FocuserTemperatureChangedEventArgs e)
        {
            if (FocuserTemperatureChanged != null)
            {
                FocuserTemperatureChanged(this, e);
            }
        }

        public virtual void OnFocuserHumidityChanged(FocuserHumidityChangedEventArgs e)
        {
            if (FocuserHumidityChanged != null)
            {
                FocuserHumidityChanged(this, e);
            }
        }
        #endregion

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialog form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public void SetupDialog()
        {
            // consider only showing the setup dialog if not connected
            // or call a different dialog if connected
            if (IsConnected)
                System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

            using (SetupDialogForm F = new SetupDialogForm(this.DriverInfo))
            {
               
                var result = F.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    WriteProfile(); // Persist device configuration values to the ASCOM Profile store
                }
            }
        }

        public ArrayList SupportedActions
        {
            get
            {
                tl.LogMessage("SupportedActions Get", "Returning empty arraylist");
                return new ArrayList();
            }
        }

        public string Action(string actionName, string actionParameters)
        {
            CheckConnected("Action");
            if (IsConnected)
            {
                serialPort.WriteLine(actionName + ":" + actionParameters);
                System.Threading.Thread.Sleep(100);
            }
            return "";
        }

        public void CommandBlind(string command, bool raw)
        {
            CheckConnected("CommandBlind");
            // Call CommandString and return as soon as it finishes
            this.CommandString(command, raw);
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBlind");
            // DO NOT have both these sections!  One or the other
        }

        public bool CommandBool(string command, bool raw)
        {
            CheckConnected("CommandBool");
            string ret = CommandString(command, raw);
            // TODO decode the return string and return true or false
            // or
            throw new ASCOM.MethodNotImplementedException("CommandBool");
            // DO NOT have both these sections!  One or the other
        }

        public string CommandString(string command, bool raw)
        {
            CheckConnected("CommandString");
            // it's a good idea to put all the low level communication with the device here,
            // then all communication calls this function
            // you need something to ensure that only one command is in progress at a time
            if (IsConnected)
            {
                serialPort.WriteLine(command + ":" + stepSize);
                System.Threading.Thread.Sleep(100);
            }

            string message = "sent " + command + ":" + stepSize;

            tl.LogMessage("command ", message);

            System.Diagnostics.Debug.WriteLine("messaage from arduino " + message);
            return message;
        }

        public void Dispose()
        {
            // Clean up the tracelogger and util objects
            tl.Enabled = false;
            tl.Dispose();
            tl = null;
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            message = SerialPort.ReadTo("#");
            System.Diagnostics.Debug.WriteLine(message);
            existingMessage = SerialPort.ReadExisting();
            System.Diagnostics.Debug.WriteLine("Existing" + existingMessage);

            if (message.Contains("POSITION")){
                focuserPosition = Convert.ToInt16(message.Split(':')[1]);
                OnFocuserValueChanged(new FocuserValueChangedEventArgs(focuserPosition, focuserPosition));
                OnFocuserStateChanged(new FocuserStateChangedEventArgs(false));
                this.isMoving = false;
            }

            if (message.Contains("MOVING"))
            {
                OnFocuserStateChanged(new FocuserStateChangedEventArgs(true));
                this.isMoving = true;
            }

            if (message.Contains("TEMPERATURE"))
            {
                this.temperature = Convert.ToDouble(message.Split(':')[1]);
                OnFocuserTemperatureChanged(
                    new FocuserTemperatureChangedEventArgs(message.Split(':')[1] + " °C"));
            }

            if (message.Contains("HUMIDITY"))
            {
                this.humidity = Convert.ToDouble(message.Split(':')[1]);
                OnFocuserHumidityChanged(
                    new FocuserHumidityChangedEventArgs(message.Split(':')[1] + "%"));
            }
        }

        public bool Connected
        {
            get
            {
                tl.LogMessage("Connected Get", IsConnected.ToString());
                return IsConnected;
            }
            set
            {
                tl.LogMessage("Connected Set", value.ToString());

                // well connect to the serial port
                ReadProfile();

                if (value == IsConnected)
                {
                    if (!mainWindow.Visible && showUI)
                    {
                        mainWindow = new MainWindow(this);
                        mainWindow.Show();
                    }
                    return;
                }
                    
                if (value)
                {
                    // Show the Window for the EQFocuser here
                    if (showUI)
                    {
                        mainWindow = new MainWindow(this);
                        mainWindow.Show();
                    }

                    connectedState = true;
                    tl.LogMessage("Connected Set", "Connecting to port " + comPort);

                    // we need to know the current position
                    serialPort = new SerialPort(comPort, 115200);
                    serialPort.DataReceived += new SerialDataReceivedEventHandler(this.serialPort_DataReceived);
                    serialPort.Open();
                    Action("F", "");    // GET POSITION
                    Action("k", "");    // GET TEMPERATURE AND HUMIDITY

                    // when we establish connection, set up the increment, step and speed
                    Action("I", "100"); // SET SPEED
                    Action("J", "200"); // SET MAXSPEED
                    Action("H", "200"); // SET ACCELERATION

                }
                else
                {
                    connectedState = false;
                    tl.LogMessage("Connected Set", "Disconnecting from port " + comPort);

                    serialPort.Close();
                    if (showUI)
                    {
                        mainWindow.Close();
                    }
                }
            }
        }

        public SerialPort SerialPort
        {
            get
            {
                return serialPort;
            }
        }

        public string Description
        {
            get
            {
                tl.LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                tl.LogMessage("InterfaceVersion Get", "2");
                return Convert.ToInt16("2");
            }
        }

        public string Name
        {
            get
            {
                string name = "Short driver name - please customise";
                tl.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region IFocuser Implementation

        private int focuserPosition = 0; // Class level variable to hold the current focuser position
        private const int focuserSteps = 10000;
        private double stepSize = 500;
        private int maxIncrement = 1000;

        public bool Absolute
        {
            get
            {
                tl.LogMessage("Absolute Get", true.ToString());
                return true; // This is an absolute focuser
            }
        }

        public void Halt()
        {
            tl.LogMessage("Halt", "Called");
            SerialPort.WriteLine("X");
        }

        public bool IsMoving
        {
            get
            {
                tl.LogMessage("IsMoving Get", false.ToString());
                return this.isMoving; // This focuser always moves instantaneously so no need for IsMoving ever to be True
            }
        }

        public bool Link
        {
            get
            {
                tl.LogMessage("Link Get", this.Connected.ToString());
                return this.Connected; // Direct function to the connected method, the Link method is just here for backwards compatibility
            }
            set
            {
                tl.LogMessage("Link Set", value.ToString());
                this.Connected = value; // Direct function to the connected method, the Link method is just here for backwards compatibility
            }
        }

        public int MaxIncrement
        {
            get
            {
                return maxIncrement;
            }
            set
            {
                maxIncrement = value;
            }
        }

        public int MaxStep
        {
            get
            {
                tl.LogMessage("MaxStep Get", focuserSteps.ToString());
                return focuserSteps; // Maximum extent of the focuser, so position range is 0 to 10,000
            }
        }

        public void Move(int Position)
        {
            tl.LogMessage("Move", Position.ToString());
            Action("E", Position.ToString());
        }

        public int Position
        {
            get
            {
                return focuserPosition; // Return the focuser position
            }
        }

        public double StepSize
        {
            get
            {
                return stepSize;
            }
            set
            {
                stepSize = value;
            }
        }

        public bool TempComp
        {
            get
            {
                tl.LogMessage("TempComp Get", false.ToString());
                return false;
            }
            set
            {
                tl.LogMessage("TempComp Set", "Not implemented");
                throw new ASCOM.PropertyNotImplementedException("TempComp", false);
            }
        }

        public bool TempCompAvailable
        {
            get
            {
                tl.LogMessage("TempCompAvailable Get", false.ToString());
                return true; // Temperature compensation is not available in this driver
            }
        }

        public double Temperature
        {
            get
            {
                tl.LogMessage("Temperature Get", "Not implemented");
                return temperature;
            }
        }

        #endregion

        #region Private properties and methods
        // here are some useful properties and methods that can be used as required
        // to help with driver development

        #region ASCOM Registration

        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        /// <summary>
        /// Register or unregister the driver with the ASCOM Platform.
        /// This is harmless if the driver is already registered/unregistered.
        /// </summary>
        /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
        private static void RegUnregASCOM(bool bRegister)
        {
            using (var P = new ASCOM.Utilities.Profile())
            {
                P.DeviceType = "Focuser";
                if (bRegister)
                {
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
                }
            }
        }

        /// <summary>
        /// This function registers the driver with the ASCOM Chooser and
        /// is called automatically whenever this class is registered for COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is successfully built.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
        /// </remarks>
        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        /// <summary>
        /// This function unregisters the driver from the ASCOM Chooser and
        /// is called automatically whenever this class is unregistered from COM Interop.
        /// </summary>
        /// <param name="t">Type of the class being registered, not used.</param>
        /// <remarks>
        /// This method typically runs in two distinct situations:
        /// <list type="numbered">
        /// <item>
        /// In Visual Studio, when the project is cleaned or prior to rebuilding.
        /// For this to work correctly, the option <c>Register for COM Interop</c>
        /// must be enabled in the project settings.
        /// </item>
        /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
        /// </list>
        /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
        /// </remarks>
        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }

        #endregion

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private bool IsConnected
        {
            get
            {
                // TODO check that the driver hardware connection exists and is connected to the hardware
                return connectedState;
            }
        }

        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        private void CheckConnected(string message)
        {
            if (!IsConnected)
            {
                throw new ASCOM.NotConnectedException(message);
            }
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Focuser";
                traceState = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
                comPort = driverProfile.GetValue(driverID, comPortProfileName, string.Empty, comPortDefault);
                showUI = Convert.ToBoolean(driverProfile.GetValue(driverID, showUIProfileName, string.Empty, showUIDefault));
            }
        }

        /// <summary>
        /// Write the device configuration to the  ASCOM  Profile store
        /// </summary>
        internal void WriteProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "Focuser";
                driverProfile.WriteValue(driverID, traceStateProfileName, traceState.ToString());
                driverProfile.WriteValue(driverID, comPortProfileName, comPort.ToString());
                driverProfile.WriteValue(driverID, showUIProfileName, showUI.ToString());
            }
        }

        #endregion

    }
}
