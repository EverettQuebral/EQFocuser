using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.EQFocuser;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.EQFocuser
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        public SetupDialogForm(string driverInfo)
        {
            InitializeComponent();
            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
            lblInfo.Text = "EQFocuser ASCOM Driver " + driverInfo;
        }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Place any validation constraint checks here
            // Update the state variables with results from the dialogue
            Focuser.comPort = (string)comboBoxComPort.SelectedItem;
            Focuser.traceState = chkTrace.Checked;
            Focuser.showUI = showUI.Checked;
        }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void InitUI()
        {
            chkTrace.Checked = Focuser.traceState;
            // set the list of com ports to those that are currently available
            comboBoxComPort.Items.Clear();

            String[] ports = SerialPort.GetPortNames();
            foreach(string port in ports)
            {
                Debug.WriteLine("Port here: " + port);
                if (DetectArduino(port))
                {
                    comboBoxComPort.Items.Add(port);
                }
            }
            //comboBoxComPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());      // use System.IO because it's static
            // select the current port if possible
            if (null != Focuser.comPort)
            {
                if (comboBoxComPort.Items.Contains(Focuser.comPort))
                {
                    comboBoxComPort.SelectedItem = Focuser.comPort;
                }
            }
            
        }

        private bool DetectArduino(string portName)
        {
            SerialPort testPort = new SerialPort(portName, 115200);
            try
            {
                testPort.Open();
                testPort.WriteLine("Z");    // command to get the name of the device

                Thread.Sleep(50);
                string returnMessage = testPort.ReadExisting().ToString();
                testPort.Close();
                Debug.WriteLine(returnMessage);

                if (returnMessage.Contains("EQFOCUSER"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch( Exception e)
            {
                return false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}