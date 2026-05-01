using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration; 
using Triathlon.Networking;
using Triathlon.Services;
using Triathlon.Server.Repository;

namespace Triathlon.Server
{
    public class StartRpcServer
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
            Console.WriteLine("Server: Se inițializează repository-urile...");

            IRefereeRepo refereeRepo = new RefereeDbRepo();
            IParticipantRepo participantRepo = new ParticipantDbRepo();
            IEventRepo eventRepo = new EventDbRepo();

            ITriathlonServices triathlonService = new TriathlonService(participantRepo, refereeRepo, eventRepo);

            int port = 55556;
            try
            {
                string portString = ConfigurationManager.AppSettings["triathlon.server.port"];
                if (!string.IsNullOrEmpty(portString))
                {
                    port = int.Parse(portString);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Server: Port invalid în config. Se folosește portul default: 55555");
            }

            TcpListener server = new TcpListener(IPAddress.Any, port);
            
            try 
            {
                server.Start();
                Console.WriteLine("Server: Pornit cu succes pe portul {0}!", port);
                Console.WriteLine("Server: Se așteaptă conexiuni de la clienți...");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Server: Client nou conectat!");

                    ClientRpcWorker worker = new ClientRpcWorker(triathlonService, client);
                    
                    Thread t = new Thread(new ThreadStart(worker.Run));
                    t.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Server Eroare Fatală: " + e.Message);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}