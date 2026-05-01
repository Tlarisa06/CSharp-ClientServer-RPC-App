using System;
using System.Windows;
using System.Configuration;
using Triathlon.Networking;
using Triathlon.Services;
using Triathlon.Client.Views; 
using Triathlon.Client.Controllers;

namespace Triathlon.Client;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
        base.OnStartup(e);

        try
        {
            string host = ConfigurationManager.AppSettings["server.host"] ?? "localhost";
            string portStr = ConfigurationManager.AppSettings["server.port"] ?? "55556";
            int port = int.Parse(portStr);

            ITriathlonServices server = new TriathlonServicesRpcProxy(host, port);

            var loginWin = new LoginWindow();

            var loginCtrl = new LoginController(server, loginWin);

            loginWin.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Eroare la pornire: {ex.Message}", "Eroare Fatală", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}