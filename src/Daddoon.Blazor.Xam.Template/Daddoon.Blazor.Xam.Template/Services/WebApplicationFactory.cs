using Daddoon.Blazor.Xam.Template.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Windows.ApplicationModel.Background;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Template.Services
{
    public sealed class WebApplicationFactory : IDisposable, IBackgroundTask
    {
        private WebServer _server { get; set; }
        private Task _execution { get; set; }

        private CancellationTokenSource cts { get; set; }

        private bool _started = false;

        public bool Start()
        {
            return Start("localhost", 46930);
        }

        public bool Start(string host, int port)
        {
            if (String.IsNullOrEmpty(host) || port < 1024)
            {
                return false;
            }

            return Start($"http://{host}:{port}/");
        }

        public bool Start(string url)
        {
            if (String.IsNullOrEmpty(url) || _started)
            {
                return false;
            }

            if (!url.EndsWith("/"))
            {
                //Ending slash is mandatory for the webserver
                url = url + "/";
            }

            // Our web server is disposable.
            _server = new WebServer(url, Unosquare.Labs.EmbedIO.Constants.RoutingStrategy.Wildcard);
            // First, we will configure our web server by adding Modules.
            // Please note that order DOES matter.
            // ================================================================================================
            // If we want to enable sessions, we simply register the LocalSessionModule
            // Beware that this is an in-memory session storage mechanism so, avoid storing very large objects.
            // You can use the server.GetSession() method to get the SessionInfo object and manupulate it.
            // You could potentially implement a distributed session module using something like Redis
            //_server.RegisterModule(new Unosquare.Labs.EmbedIO.Modules.LocalSessionModule());

            //Get BaseURI folder
            string baseUrl = DependencyService.Get<IBaseUrl>().Get();

            // Here we setup serving of static files
            _server.RegisterModule(new Unosquare.Labs.EmbedIO.Modules.StaticFilesModule(baseUrl));
            // The static files module will cache small files in ram until it detects they have been modified.
            _server.Module<Unosquare.Labs.EmbedIO.Modules.StaticFilesModule>().UseRamCache = true;
            _server.Module<Unosquare.Labs.EmbedIO.Modules.StaticFilesModule>().DefaultExtension = ".html";

            // We don't need to add the line below. The default document is always index.html.
            //server.Module<modules.staticfileswebmodule>().DefaultDocument = "index.html";

            cts = new CancellationTokenSource();

            // Once we've registered our modules and configured them, we call the RunAsync() method.
            _execution = _server.RunAsync(cts.Token);

            return true;
        }

        public void Dispose()
        {
            cts.Cancel();
            try
            {
                _execution.Wait();
            }
            catch (AggregateException)
            {
                _execution = null;
                _server?.Dispose();
                _server = null;
                _started = false;
            }
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            var url = "http://localhost:46930/";

            if (String.IsNullOrEmpty(url) || _started)
            {
                return;
            }

            if (!url.EndsWith("/"))
            {
                //Ending slash is mandatory for the webserver
                url = url + "/";
            }

            // Our web server is disposable.
            _server = new WebServer(url, Unosquare.Labs.EmbedIO.Constants.RoutingStrategy.Wildcard);
            // First, we will configure our web server by adding Modules.
            // Please note that order DOES matter.
            // ================================================================================================
            // If we want to enable sessions, we simply register the LocalSessionModule
            // Beware that this is an in-memory session storage mechanism so, avoid storing very large objects.
            // You can use the server.GetSession() method to get the SessionInfo object and manupulate it.
            // You could potentially implement a distributed session module using something like Redis
            //_server.RegisterModule(new Unosquare.Labs.EmbedIO.Modules.LocalSessionModule());

            //Get BaseURI folder
            string baseUrl = DependencyService.Get<IBaseUrl>().Get();

            // Here we setup serving of static files
            _server.RegisterModule(new Unosquare.Labs.EmbedIO.Modules.StaticFilesModule(baseUrl));
            // The static files module will cache small files in ram until it detects they have been modified.
            _server.Module<Unosquare.Labs.EmbedIO.Modules.StaticFilesModule>().UseRamCache = true;
            _server.Module<Unosquare.Labs.EmbedIO.Modules.StaticFilesModule>().DefaultExtension = ".html";

            // We don't need to add the line below. The default document is always index.html.
            //server.Module<modules.staticfileswebmodule>().DefaultDocument = "index.html";

            cts = new CancellationTokenSource();

            // Once we've registered our modules and configured them, we call the RunAsync() method.
            _execution = _server.RunAsync(cts.Token);

            deferral.Complete();
        }
    }
}
