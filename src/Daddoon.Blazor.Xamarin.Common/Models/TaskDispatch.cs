using Daddoon.Blazor.Xam.Common.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daddoon.Blazor.Xam.Common.Models
{
    public class TaskDispatch
    {
        public int TaskId { get; set; }

        public Task ResultAction { get; set; }

        public CancellationTokenSource CancelTokenSource { get; set; }

        public CancellationToken CancelToken { get; set; }

        public MethodProxy ResultData { get; set; }

        public void CancelTask()
        {
            CancelTokenSource.Cancel();
        }
    }
}
