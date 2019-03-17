using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BlazorMobile.Common.Helpers
{
    internal static class InternalHelper
    {
        public static void SetTimeout(Action handler, int timeout)
        {
            ElapsedEventHandler tmElapsed = null;
            Timer tm = new Timer(timeout);

            tmElapsed = delegate (object sender, ElapsedEventArgs e)
            {
                tm.Stop();
                tm.Elapsed -= tmElapsed;
                tm.Dispose();

                handler();
            };


            tm.Elapsed += tmElapsed;
            tm.Start();
        }
    }
}
