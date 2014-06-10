using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace PresenceTracker
{
    public enum State
    {
        Shutdown,
        Logon,
        Logoff,
        Lock,
        Unlock,
        Unknown,
        AppStart,
        Resume,
        Suspend
    }
}
