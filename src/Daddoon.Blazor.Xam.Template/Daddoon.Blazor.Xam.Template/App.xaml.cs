using Daddoon.Blazor.Xam.Template.Services;
using System;
using Windows.ApplicationModel.Background;
using Xamarin.Forms;

namespace Daddoon.Blazor.Xam.Template
{
    public class BackgroundTasksFactory
    {
        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint, string name, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {

                if (cur.Value.Name == name)
                    // The task is already registered.
                    return (BackgroundTaskRegistration)(cur.Value);

            }

            //Register new background task:
            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            return task;
        }
    }

    public partial class App : Application
	{
        public WebApplicationFactory webApplication { get; set; }

        public App ()
		{
			InitializeComponent();

            BackgroundTasksFactory.RegisterBackgroundTask("Daddoon.Blazor.Xam.Template.Services.WebApplicationFactory", "MyBackgroundTask", new SystemTrigger(SystemTriggerType.InternetAvailable, false), null);
            //webApplication = new WebApplicationFactory();

			MainPage = new MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
