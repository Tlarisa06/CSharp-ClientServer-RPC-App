using System;
using System.Windows;
using Triathlon.Services;
using Triathlon.Model;
using Triathlon.Client.Views;

namespace Triathlon.Client.Controllers
{
    public class LoginController
    {
        private ITriathlonServices service;
        private LoginWindow view;

        public LoginController(ITriathlonServices service, LoginWindow view)
        {
            this.service = service;
            this.view = view;
            this.view.LoginButton.Click += HandleLogin;
        }

        private void HandleLogin(object sender, RoutedEventArgs e)
        {
            string user = view.UsernameField.Text;
            string pass = view.PasswordField.Password;

            try
            {
                var mainWin = new MainWindow();
                var mainCtrl = new MainController(service, mainWin);

                Referee referee = service.Login(user, pass, mainCtrl);

                if (referee != null)
                {
                    mainCtrl.SetReferee(referee);
                    mainWin.SetController(mainCtrl);
                    mainWin.Show();
                    view.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
