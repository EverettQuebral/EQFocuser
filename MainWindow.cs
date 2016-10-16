using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            InitializeComponent();
            InitControls();
        }

        delegate void SetCurrentPositionCallBack(int position);
        delegate void SetCurrentStateCallBack(bool isMoving);
        delegate void SetCurrentTemperatureCallBack(string temperature);
        delegate void SetCurrentHumidityCallBack(string humidity);

        private void SetCurrentTemperature(string temperature)
        {
            if (txtBoxTemperature.InvokeRequired)
            {
                SetCurrentTemperatureCallBack d = new SetCurrentTemperatureCallBack(SetCurrentTemperature);
                this.Invoke(d, new Object[] { temperature });
            }
            else
            {
                txtBoxTemperature.Text = temperature.ToString();
            }
        }

        private void SetCurrentHumidity(string humidity)
        {
            if (txtBoxHumidity.InvokeRequired)
            {
                SetCurrentHumidityCallBack d = new SetCurrentHumidityCallBack(SetCurrentHumidity);
                this.Invoke(d, new Object[] { humidity });
            }
            else
            {
                txtBoxHumidity.Text = humidity.ToString();
            }
        }

        private void SetCurrentPosition(int position)
        {
            if (textBoxCurrentPosition.InvokeRequired)
            {
                SetCurrentPositionCallBack d = new SetCurrentPositionCallBack(SetCurrentPosition);
                this.Invoke(d, new object[] { position });
            }
            else
            {
                textBoxCurrentPosition.Text = position.ToString();
            }

            if (!focuser.IsMoving) lblAction.Text = "READY...";
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

        private void btnFastReverse_Click(object sender, EventArgs e)
        {
            focuser.CommandString("A", true);
        }

        private void InitControls()
        {
            textBoxCurrentPosition.Text = focuser.Position.ToString();
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
                focuser.CommandString("k", true);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Stopping the motor");
            focuser.CommandString("X", true);
            focuser.Halt();
        }
    }
}
