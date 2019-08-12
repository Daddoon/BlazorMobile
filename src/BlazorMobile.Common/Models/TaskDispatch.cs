using BlazorMobile.Common.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorMobile.Common.Models
{
    internal class TaskDispatch
    {
        public int TaskId { get; set; }

        public Task ResultAction { get; set; }

        public CancellationTokenSource CancelTokenSource { get; set; }

        public CancellationToken CancelToken { get; set; }

        public MethodProxy ResultData { get; set; }

        private bool _isFaulted = false;

        private Exception _exception = null;

        public void ThrowExceptionIfFaulted()
        {
            if (_isFaulted)
            {
                //Let's bubble up exception
                throw _exception;
            }
        }

        public void SetTaskAsFaulted<T>(T ex) where T : Exception
        {
            _exception = ex;
            _isFaulted = true;
        }
    }
}
