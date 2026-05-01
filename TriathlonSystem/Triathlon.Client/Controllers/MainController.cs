using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Triathlon.Model;
using Triathlon.Model.DTO;
using Triathlon.Services;
using Triathlon.Client.Views;

namespace Triathlon.Client.Controllers
{
    public class MainController : ITriathlonObserver
    {
        private ITriathlonServices service;
        private MainWindow view;
        private Referee loggedReferee;
        public ObservableCollection<ParticipantDTO> Participants { get; set; } = new ObservableCollection<ParticipantDTO>();
        public ObservableCollection<ParticipantDTO> Report { get; set; } = new ObservableCollection<ParticipantDTO>();

        public MainController(ITriathlonServices service, MainWindow view)
        {
            this.service = service;
            this.view = view;
            this.view.DataContext = this; 
        }

        public void SetReferee(Referee referee)
        {
            this.loggedReferee = referee;
            this.view.WelcomeLabel.Content = "Arbitru: " + GetLoggedRefereeName();
            LoadData();
        }

        public string GetLoggedRefereeName()
        {
            return loggedReferee?.Name ?? "Necunoscut";
        }

        public void LoadData()
        {
            try
            {
                var all = service.GetParticipantsByEvent(-1);
                var reportData = service.GetParticipantsByEvent(loggedReferee.IdEvent);

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Participants.Clear();
                    foreach (var p in all) Participants.Add(p);

                    Report.Clear();
                    foreach (var p in reportData) Report.Add(p);
                    
                    view.AllParticipantsTable.Items.Refresh();
                    view.ReportTable.Items.Refresh();
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Eroare la încărcarea datelor: " + ex.Message);
            }
        }

        public void AddResult(ParticipantDTO participant, string pointsStr)
        {
            if (participant == null || !int.TryParse(pointsStr, out int points))
            {
                MessageBox.Show("Vă rugăm selectați un participant și introduceți un punctaj valid!");
                return;
            }

            try
            {
                service.AddResult(loggedReferee.Id, participant.IdParticipant, points);
                
                view.PointsField.Text = "";
                
                LoadData();

                MessageBox.Show("Rezultat salvat cu succes!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la salvare: " + ex.Message);
            }
        }

        public void Logout()
        {
            try 
            { 
                service.Logout(loggedReferee, this); 
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("Logout error: " + ex.Message); 
            }
        }

        public void UpdateReceived()
        {
             Application.Current.Dispatcher.BeginInvoke(new Action(() => LoadData()));
        }
    }
}