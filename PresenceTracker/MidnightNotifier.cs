using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PresenceTracker
{
    // with modifications taken from Ben Hoffstein (http://stackoverflow.com/a/8480218/1804734)
    static class MidnightNotifier
    {
        private static readonly Timer timer;
        public static event EventHandler<EventArgs> DayChanged;

        static MidnightNotifier()
        {
            timer = new Timer(GetSleepTime());
            timer.Elapsed += (s, e) =>
                {
                    DayChanged.Raise(null, null);
                    timer.Interval = GetSleepTime();
                };
            timer.Start();

            SystemEvents.TimeChanged += OnSystemTimeChanged;
        }

        private static double GetSleepTime()
        {
            var midnightTonight = DateTime.Today.AddDays(1);
            var differenceInMilliseconds = (midnightTonight - DateTime.Now).TotalMilliseconds;
            return differenceInMilliseconds;
        }

        private static void OnSystemTimeChanged(object sender, EventArgs e)
        {
            timer.Interval = GetSleepTime();
        }
    }
}