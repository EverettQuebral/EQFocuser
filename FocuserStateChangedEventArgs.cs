using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASCOM.EQFocuser
{
    public class FocuserStateChangedEventArgs : EventArgs
    {
        public readonly bool IsMoving;
        
        public FocuserStateChangedEventArgs(bool isMoving)
        {
            this.IsMoving = isMoving;
        }
    }
}
