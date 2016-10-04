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
            InitializeComponent();
            this.focuser = focuser;
        }

        private void btnFastReverse_Click(object sender, EventArgs e)
        {
            focuser.CommandString("FASTFORWARD", true);
        }
    }
}
