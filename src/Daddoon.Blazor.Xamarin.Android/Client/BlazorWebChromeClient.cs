using Android.Webkit;

public class BlazorWebChromeclient : WebChromeClient
{
    public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
    {
        Android.Util.Log.Debug("WebView", consoleMessage.Message());
        return true;
    }
}