using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Triathlon.Model;
using Triathlon.Model.DTO;
using Triathlon.Services;
using Triathlon.Server.Repository;

namespace Triathlon.Server
{
    public class TriathlonService : ITriathlonServices
    {
        private readonly IParticipantRepo _participantRepo;
        private readonly IRefereeRepo _refereeRepo;
        private readonly IEventRepo _eventRepo;

        private readonly IDictionary<int, ITriathlonObserver> _loggedClients;

        public TriathlonService(IParticipantRepo pRepo, IRefereeRepo rRepo, IEventRepo eRepo)
        {
            _participantRepo = pRepo;
            _refereeRepo = rRepo;
            _eventRepo = eRepo;
            _loggedClients = new ConcurrentDictionary<int, ITriathlonObserver>();
        }

        public virtual Referee Login(string username, string password, ITriathlonObserver client)
        {
            lock (this)
            {
                Referee referee = _refereeRepo.FindByUsernameAndPassword(username, password);
                if (referee != null)
                {
                    if (_loggedClients.ContainsKey(referee.Id))
                    {
                        throw new Exception("Utilizator deja logat.");
                    }

                    _loggedClients[referee.Id] = client;
                    Console.WriteLine($"Service: Arbitrul {referee.Name} s-a logat.");
                    return referee;
                }
                else
                {
                    throw new Exception("Autentificare eșuată! Username sau parolă incorectă.");
                }
            }
        }

        public virtual void Logout(Referee referee, ITriathlonObserver client)
        {
            lock (this)
            {
                if (referee == null)
                {
                    var entry = _loggedClients.FirstOrDefault(x => x.Value == client);
                    if (entry.Value != null)
                    {
                        _loggedClients.Remove(entry.Key);
                        Console.WriteLine($"Service: Client deconectat (căutat după observer).");
                    }
                }
                else
                {
                    bool removed = _loggedClients.Remove(referee.Id);
                    if (!removed) throw new Exception("Utilizatorul nu este logat.");
                    Console.WriteLine($"Service: Arbitrul {referee.Name} s-a delogat.");
                }
            }
        }

        public virtual void AddResult(int idReferee, int idParticipant, int points)
        {
            if (points < 0) throw new Exception("Punctajul nu poate fi negativ!");

            lock (this)
            {
                _eventRepo.RemoveAllByRefereeAndParticipant(idReferee, idParticipant);

                Event newEvent = new Event(0, idReferee, idParticipant, points);
                _eventRepo.Add(newEvent);

                Participant p = _participantRepo.FindById(idParticipant);
                if (p != null)
                {
                    int totalSuma = _eventRepo.GetTotalPointsForParticipant(idParticipant);
                    p.TotalPoints = totalSuma;
                    _participantRepo.Update(p);
            
                    Console.WriteLine($"[SERVICE] Scor actualizat pentru {p.Name}. Nou total: {p.TotalPoints}");
                }

                NotifyClients();
            }
        }

        private void NotifyClients()
        {
            Console.WriteLine("Service: Notificăm clienții despre update...");
            
            foreach (var client in _loggedClients.Values)
            {
                Task.Run(() =>
                {
                    try
                    {
                        client.UpdateReceived();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Service ERROR: Eroare la notificarea unui client: {e.Message}");
                    }
                });
            }
        }

        public virtual List<ParticipantDTO> GetParticipantsByEvent(int idEvent)
        {
            lock (this)
            {
                var allParticipants = _participantRepo.GetAll().ToList();

                if (idEvent == -1)
                {
                    Console.WriteLine("Service: Trimit lista alfabetica.");
                    return allParticipants
                        .OrderBy(p => p.Name)
                        .Select(p => new ParticipantDTO(p.Id, p.Name, p.TotalPoints))
                        .ToList();
                }
                else
                {
                    Console.WriteLine($"Service: Trimit clasament pentru proba.");
                    return allParticipants
                        .OrderByDescending(p => p.TotalPoints)
                        .Select(p => new ParticipantDTO(p.Id, p.Name, p.TotalPoints))
                        .ToList();
                }
            }
        }

        public virtual List<EventDTO> GetAllEventsDTO()
        {
            lock (this)
            {
                var allEvents = _eventRepo.GetAll().ToList();
                return allEvents.Select(e => new EventDTO(e.Id, "Proba " + e.Id, e.IdRef, e.IdPart, e.Points)).ToList();
            }
        }
    }
}