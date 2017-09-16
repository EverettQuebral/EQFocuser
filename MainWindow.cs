using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;


namespace ASCOM.EQFocuser
{
    public partial class MainWindow : Form
    {
        ASCOM.EQFocuser.Focuser focuser;
        public MainWindow(Focuser focuser)
        {
            this.focuser = focuser;
            this.focuser.FocuserValueChanged += FocuserValueChanged;
            this.focuser.FocuserStateChanged += FocuserStateChanged;
            this.focuser.FocuserHumidityChanged += FocuserHumidityChanged;
            this.focuser.FocuserTemperatureChanged += FocuserTemperatureChanged;
            this.focuser.FocuserMotorChanged += FocuserMotorChanged;
            InitializeComponent();
            InitControls();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

        }

        delegate void SetCurrentPositionCallBack(int position);
        delegate void SetCurrentStateCallBack(bool isMoving);
        delegate void SetCurrentTemperatureCallBack(string temperature);
        delegate void SetCurrentHumidityCallBack(string humidity);
        delegate void SetCurrentMotorCallBack(int motorNumber);

        private void SetCurrentTemperature(string temperature)
        {
            txtBoxTemperature.InvokeIfRequired(txtBoxTemperature => { txtBoxTemperature.Text = temperature; });
        }

        private void SetCurrentHumidity(string humidity)
        {

            txtBoxHumidity.InvokeIfRequired(txtBoxHumidity => { txtBoxHumidity.Text = humidity; });
        }

        private void SetCurrentPosition(int position)
        {
            textBoxCurrentPosition.InvokeIfRequired(textBoxCurrentPosition => { textBoxCurrentPosition.Text = position.ToString(); });
        }

        private void FocuserValueChanged(object sender, FocuserValueChangedEventArgs e)
        {
            //this.textBoxCurrentPosition.Text = e.NewValue.ToString();
            SetCurrentPosition(e.NewValue);
        }

        private void SetCurrentState(bool isMoving)
        {
            if (lblAction.InvokeRequired)
            {
                SetCurrentStateCallBack d = new SetCurrentStateCallBack(SetCurrentState);
                this.Invoke(d, new Object[] { isMoving });
            }
            else
            {
                if (isMoving)
                {
                    lblAction.Text = "MOVING...";
                }
                else
                {
                    lblAction.Text = "READY...";
                }
               
            }
        }

        private void SetCurrentMotor(int motorNumber)
        {
            if (checkBox1.InvokeRequired || checkBox2.InvokeRequired)
            {
                SetCurrentMotorCallBack d = new SetCurrentMotorCallBack(SetCurrentMotor);
                this.Invoke(d, new Object[] { motorNumber });
            }
            else
            {
                if (motorNumber == 0)
                {
                    checkBox1.Checked = true;
                    checkBox2.Checked = false;
                }
                if (motorNumber == 1)
                {
                    checkBox1.Checked = false;
                    checkBox2.Checked = true;
                }
            }
        }

        private void FocuserHumidityChanged(object sender, FocuserHumidityChangedEventArgs e)
        {
            SetCurrentHumidity(e.Humidity);
        }

        private void FocuserTemperatureChanged(object sender, FocuserTemperatureChangedEventArgs e)
        {
            SetCurrentTemperature(e.Temperature);
        }

        private void FocuserStateChanged(object sender, FocuserStateChangedEventArgs e)
        {
            SetCurrentState(e.IsMoving);
        }

        private void FocuserMotorChanged(object sender, FocuserMotorChangedEventArgs e)
        {
            SetCurrentMotor(e.Motor);
        }

        private void btnFastReverse_Click(object sender, EventArgs e)
        {
            focuser.CommandString("A", true);
        }

        private void InitControls()
        {
            textBoxCurrentPosition.Text = focuser.Position.ToString();
            checkBox1.Checked = true;
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {

            focuser.CommandString("B", true);
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            focuser.CommandString("C", true);
        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            focuser.CommandString("D", true);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            focuser.StepSize = Convert.ToDouble(numericUpDown1.Value);
        }

        private void btnMoveTo_Click(object sender, EventArgs e)
        {
            focuser.Move(Convert.ToInt16(textBoxMoveToPosition.Text));
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            focuser.CommandString("G", true);
            SetCurrentPosition(0);
        }

        private void textBoxCurrentPosition_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            // acceleration
            focuser.Action("H", numericUpDown2.Value.ToString());
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            // max speed
            focuser.Action("J", numericUpDown3.Value.ToString());
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            // speed
            focuser.Action("I", numericUpDown4.Value.ToString());
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // do not disrupt the motor when it is moving
            if (!focuser.IsMoving) {
                System.Diagnostics.Debug.WriteLine("Getting Temperature");
                focuser.Action("k", "");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Stopping the motor");
            focuser.Halt();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Showing Main Window");
            System.Diagnostics.Debug.WriteLine("Height " + this.Height);
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = "EQFocuser v" + String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", version.Major, version.Minor, version.Revision);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //focuser.SerialPort.Close();
        }

        private void numericUpDownBacklash_ValueChanged(object sender, EventArgs e)
        {
            focuser.Action("L", numericUpDownBacklash.Value.ToString());
        }


        private void btnShowAdvanced_Click(object sender, EventArgs e)
        {
            //panel1.Visible = !panel1.Visible;

            //if (panel1.Visible)
            //{
            //    this.Height = 665;
            //}
            //else
            //{
            //    this.Height = 376;
            //}

            txtBoxTemperature.Text = this.Height.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox1.Checked;
            focuser.Action("M", "0");
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = !checkBox2.Checked;
            focuser.Action("M", "1");
        }

        private void lblAction_MouseClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(this.Height.ToString());
        }

        private void checkBoxReverse_CheckedChanged(object sender, EventArgs e)
        {
            focuser.CommandBool("Y", checkBoxReverse.Checked);
        }
    }
}
