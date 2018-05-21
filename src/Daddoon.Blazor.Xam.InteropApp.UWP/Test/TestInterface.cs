using Daddoon.Blazor.Xam.Common.Interop;
using Daddoon.Blazor.Xam.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Daddoon.Blazor.Xam.InteropApp.UWP.Test
{
    public interface IClass
    {
        void Test(int nb);
        void Test2(int nb);

        void Test3<T>(int nb);
    }

    public interface IClassSecond
    {
        void Test(int nb);
        void Test2(int nb);

        void Test3<T>(int nb);
    }

    public class ClassB : IClass
    {
        public void Test(int nb)
        {
            Console.WriteLine("ClassB Test");
        }

        public void Test2(int nb)
        {
            Console.WriteLine("ClassB Test2");
        }

        public void Test3<T>(int nb)
        {
            Console.WriteLine("Generic call");
        }
    }

    public static class TestMethods
    {
        public static void Test()
        {
            var a = DependencyService.Get<IClass>();
            var asecond = DependencyService.Get<IClassSecond>();
            a.Test3<float>(5);
            a.Test(5);
            //var a = new ClassA();
            var b = new ClassB();

            //Get implemented A
            var methodInfoA = SymbolExtensions.GetMethodInfo(() => a.Test(5));

            //Get interface IClass from A
            //var imethodInfo = MethodProxyHelper.GetInterfaceMethodIndex(a.GetType(), methodInfoA, typeof(IClass));

            ////Get implemented B from interface IClass from A
            //var imethodb = MethodProxyHelper.GetClassMethodInfo(b.GetType(), typeof(IClass), imethodInfo);

            //imethodb.Invoke(b, new object[] { 5 });
        }
    }
}
