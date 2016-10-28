using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASCOM.EQFocuser
{
    public static class ExtensionMethods
    {
        public static void InvokeIfRequired(this TextBox control, Action<TextBox> action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => action(control)));
            }
            else
            {
                action(control);
            }
        }
    }
}
