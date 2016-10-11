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
            InitializeComponent();
            InitControls();
        }

        delegate void SetCurrentPositionCallBack(int position);
        delegate void SetCurrentStateCallBack(bool isMoving);

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
    }
}
