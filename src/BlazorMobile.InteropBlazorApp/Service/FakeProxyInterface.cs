using BlazorMobile.Common.Attributes;
using BlazorMobile.InteropBlazorApp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxParser.Test
{
    [ProxyInterface]
    public interface MyInterface
    {
        System.Threading.Tasks.Task<bool> MyFirstMethod<TObject>(string text, bool value) where TObject : new();

        System.Threading.Tasks.Task MySecondMethod<TObject>(string text, bool value) where TObject : new();

        Task MyFifthMethod<TObject>() where TObject : new();

        /******************/

        Task<bool> MyFirstMethod(string text, bool value);

        Task MySecondMethod(string text, bool value);

        Task MyFifthMethod();

        //Unsupported as its not Task / Async
        MobileApp MyThirdMethod();
    }

    [ProxyInterface]
    //Should be taken
    public interface MySecondInterface
    {
        Task<bool> MyFirstMethod<TObject>(string text, bool value) where TObject : new();

        Task MySecondMethod<TObject>(string text, bool value) where TObject : new();

        Task MyFifthMethod<TObject>() where TObject : new();

        /******************/

        Task<bool> MyFirstMethod(string text, bool value);

        Task MySecondMethod(string text, bool value);

        Task MyFifthMethod();

        //Unsupported as its not Task / Async
        MobileApp MyThirdMethod();
    }

    //Should be ignored
    public interface MyThirdInterface
    {
        Task<bool> MyFirstMethod<TObject>(string text, bool value) where TObject : new();

        Task MySecondMethod<TObject>(string text, bool value) where TObject : new();

        Task MyFifthMethod<TObject>() where TObject : new();

        /******************/

        Task<bool> MyFirstMethod(string text, bool value);

        Task MySecondMethod(string text, bool value);

        Task MyFifthMethod();

        //Unsupported as its not Task / Async
        MobileApp MyThirdMethod();
    }
}

namespace SyntaxParser.TestSecond
{
    [ProxyInterface]
    //Should be taken
    public interface MyInterface
    {
        Task<bool> MyFirstMethod<TObject>(string text, bool value) where TObject : new();

        Task MySecondMethod<TObject>(string text, bool value) where TObject : new();

        Task MyFifthMethod<TObject>() where TObject : new();

        /******************/

        Task<bool> MyFirstMethod(string text, bool value);

        Task MySecondMethod(string text, bool value);

        Task MyFifthMethod();

        //Unsupported as its not Task / Async
        MobileApp MyThirdMethod();
    }
}

//Should be taken
[ProxyInterface]
public interface MyInterface
{
    Task<bool> MyFirstMethod<TObject>(string text, bool value) where TObject : new();

    Task MySecondMethod<TObject>(string text, bool value) where TObject : new();

    Task MyFifthMethod<TObject>() where TObject : new();

    /******************/

    Task<bool> MyFirstMethod(string text, bool value);

    Task MySecondMethod(string text, bool value);

    Task MyFifthMethod();

    //Unsupported as its not Task / Async
    MobileApp MyThirdMethod();
}