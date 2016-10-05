using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASCOM.EQFocuser
{
    public class FocuserValueChangedEventArgs : EventArgs
    {
        public readonly int LastValue;
        public readonly int NewValue;
           
        public FocuserValueChangedEventArgs(int LastValue, int NewValue)
        {
            this.LastValue = LastValue;
            this.NewValue = NewValue;
        }
    }
}
