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
            tm.AutoReset = false;

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

        private const int InfiniteRetry = -1;

        public static void SetInterval(Action handler, int interval, Func<bool> stopCondition = null, int maxRetry = InfiniteRetry)
        {
            int currentInterval = -1;

            Action resetAction = null;
            resetAction = () =>
            {
                SetTimeout(() => {
                    handler();

                    currentInterval++;

                    if (maxRetry == InfiniteRetry)
                    {
                        //Call this delegate again
                        resetAction();
                    }
                    else if (stopCondition != null && stopCondition() == false && currentInterval < maxRetry)
                    {
                        //If the condition is not met and the current interval is lesser than the max interval, call again
                        resetAction();
                    }
                    else if (currentInterval < maxRetry)
                    {
                        //Same as previous, but we have no delegate checking
                        resetAction();
                    }
                }, interval);
            };

            resetAction();
        }

        public static void SetConditionalIntervalDelegate(Action handler, int interval, Func<bool> delegateCondition = null, int maxRetry = InfiniteRetry)
        {
            int currentInterval = -1;

            Action resetAction = null;
            resetAction = () =>
            {
                SetTimeout(() => {

                    //Sanity check
                    if (currentInterval == int.MaxValue)
                        currentInterval = 0;

                    currentInterval++;

                    if (delegateCondition != null && delegateCondition() == false && currentInterval < maxRetry)
                    {
                        //If the condition is not met and the current interval is lesser than the max interval, call again
                        resetAction();
                    }
                    else if (delegateCondition == null && currentInterval < maxRetry)
                    {
                        //Same as previous, but we have no delegate checking
                        resetAction();
                    }
                    else if (delegateCondition != null && delegateCondition() == true || currentInterval >= maxRetry)
                    {
                        //Call the handler
                        handler();
                    }
                }, interval);
            };

            resetAction();
        }
    }
}
