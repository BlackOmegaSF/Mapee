﻿using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Mapper.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptions;
            InitializeBackgroundWork();

            UpdateChecker.Check();

            Controller.Controller controller = new Controller.Controller();
            controller.MainWindow.ShowDialog();
        }

        private static void UnhandledExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            Directory.CreateDirectory("Crashes");
            string fileName = $"Crashes\\{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.txt";

            StringBuilder text = new StringBuilder();
            if (e.ExceptionObject is Exception exception)
            {
                text.AppendLine(exception.Message);
                text.Append(exception.StackTrace);
            }

            File.WriteAllText(fileName, text.ToString());
        }
        
        private static void InitializeBackgroundWork()
        {
            BackgroundWork.Run(TimeSpan.FromSeconds(5), () =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            });
        }
    }
}
