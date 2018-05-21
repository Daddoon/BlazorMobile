using Daddoon.Blazor.Xam.Common.Serialization;
using Daddoon.Blazor.Xam.Common.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Daddoon.Blazor.Xam.Common.Interop
{
    public class ClassProxy : DispatchProxy
    {
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            MethodProxy methodProxy = new MethodProxy();

            //As we are using a DispatchProxy, the targetMethod should be the interface MethodInfo (and not a class MethodInfo)
            int index = MethodProxyHelper.GetInterfaceMethodIndexFromGenericProxy(targetMethod, GetType());

            if (index == -1)
                return default;

            methodProxy.MethodIndex = index;
            methodProxy.GenericTypes = MethodProxyHelper.GetGenericTypesFromRuntimeMethod(targetMethod);

            //On this case, the DeclaringType should be the Interface type
            methodProxy.InterfaceType = new TypeProxy(targetMethod.DeclaringType);

            //We will let the serializer do the thing for RunTime values
            methodProxy.Parameters = args;

            var jsonMethodProxy = BridgeSerializer.Serialize(methodProxy);

            switch (ContextHelper.GetExecutingContext())
            {
                case Models.ExecutingContext.Blazor:
                    return BlazorToXamarinDispatcher.Send(jsonMethodProxy);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
