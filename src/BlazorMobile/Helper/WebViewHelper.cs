﻿using BlazorMobile.Components;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

[assembly: InternalsVisibleTo("BlazorMobile.Android")]
[assembly: InternalsVisibleTo("BlazorMobile.iOS")]
[assembly: InternalsVisibleTo("BlazorMobile.UWP")]
[assembly: InternalsVisibleTo("BlazorMobile.ElectronNET")]
namespace BlazorMobile.Helper
{
    internal static class WebViewHelper
    {
        private static readonly List<Tuple<IWebViewIdentity, List<int>>> _webviewList = new List<Tuple<IWebViewIdentity, List<int>>>();

        internal static IWebViewIdentity WebViewRequirementCheck(IBlazorWebView webview)
        {
            if (webview == null)
            {
                throw new ArgumentNullException(nameof(webview));
            }
            else if (webview is IWebViewIdentity webIdentity)
            {
                return webIdentity;
            }
            else
            {
                throw new InvalidOperationException($"{nameof(webview)} must inherit from {nameof(IBlazorWebView)} and {nameof(IWebViewIdentity)}");
            }
        }

        internal static void RegisterWebView(IBlazorWebView webview)
        {
            IWebViewIdentity identity = WebViewRequirementCheck(webview);

            try
            {
                if (!_webviewList.Any(p => p.Item1.GetWebViewIdentity() == identity.GetWebViewIdentity()))
                {
                    _webviewList.Add(new Tuple<IWebViewIdentity, List<int>>(identity, new List<int>()));
                }
            }
            catch (Exception)
            {
                //Silently ignore
                //But enforce IWebViewIdentity throwing
            }
        }

        /// <summary>
        /// Register the Webview with the specified runime identity.
        /// If the webview is already registered, it will just
        /// add an additional identity to the current webview identity list
        /// </summary>
        /// <param name="webview"></param>
        /// <param name="extraIdentity"></param>
        internal static void RegisterWebView(IBlazorWebView webview, int extraIdentity)
        {
            IWebViewIdentity webviewIdentity = WebViewRequirementCheck(webview);

            try
            {
                Tuple<IWebViewIdentity, List<int>> foundTuple = _webviewList.FirstOrDefault(p => p.Item1.GetWebViewIdentity() == webviewIdentity.GetWebViewIdentity());

                if (foundTuple == null)
                {
                    foundTuple = new Tuple<IWebViewIdentity, List<int>>(webviewIdentity, new List<int>());
                    _webviewList.Add(foundTuple);
                }

                if (!foundTuple.Item2.Contains(extraIdentity))
                {
                    foundTuple.Item2.Add(extraIdentity);
                }
            }
            catch (Exception)
            {
                //Silently ignore
                //But enforce IWebViewIdentity throwing
            }
        }

        /// <summary>
        /// Unregister the Webview instance. This must be mainly used at disposal time of the webview
        /// </summary>
        /// <param name="webview"></param>
        internal static void UnregisterWebView(IBlazorWebView webview)
        {
            IWebViewIdentity identity = WebViewRequirementCheck(webview);

            try
            {
                int index = _webviewList.FindIndex(p => p.Item1.GetWebViewIdentity() == identity.GetWebViewIdentity());
                if (index < 0)
                {
                    return;
                }

                _webviewList.RemoveAt(index);
            }
            catch (Exception)
            {
                //Silently ignore
                //But enforce IWebViewIdentity throwing
            }
        }

        /// <summary>
        /// Return the WebView associated with the given identity
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        internal static IBlazorWebView GetWebViewByRuntimeIdentity(int identity)
        {
            try
            {
                return _webviewList.FirstOrDefault(p => p.Item2.Contains(identity))?.Item1 as IBlazorWebView;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static int _counter = 0;

        /// <summary>
        /// Generate an unique identity for IWebViewIdentity
        /// </summary>
        /// <returns></returns>
        internal static int GenerateWebViewIdentity()
        {
            return _counter++;
        }
    }
}