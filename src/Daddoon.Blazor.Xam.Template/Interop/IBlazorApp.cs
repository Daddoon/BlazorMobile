using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Daddoon.Blazor.Xam.Template.Interop
{
    public interface IBlazorApp
    {
        Stream GetStream();
    }
}
