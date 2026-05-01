using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using MessagePack;
using Triathlon.Model;
using Triathlon.Model.DTO;
using Triathlon.Services;

namespace Triathlon.Networking
{
    public class TriathlonServicesRpcProxy : ITriathlonServices
    {
        private string host;
        private int port;
        private ITriathlonObserver client;
        private NetworkStream stream;
        private TcpClient connection;
        private Queue<Response> responses;
        private volatile bool finished;
        private EventWaitHandle _waitHandle = new AutoResetEvent(false);

        public TriathlonServicesRpcProxy(string host, int port)
        {
            this.host = host;
            this.port = port;
            this.responses = new Queue<Response>();
        }

        public virtual Referee Login(string username, string password, ITriathlonObserver client)
        {
            InitializeConnection();
            UserDTO userDto = new UserDTO(username, password);
            SendRequest(new Request(RequestType.LOGIN, userDto));
            
            Response response = ReadResponse();
            if (response.Type == ResponseType.OK)
            {
                this.client = client;
                return MessagePackSerializer.Deserialize<Referee>(MessagePackSerializer.Serialize(response.Data, MessagePack.Resolvers.ContractlessStandardResolver.Options));
            }
            if (response.Type == ResponseType.ERROR)
            {
                CloseConnection();
                throw new Exception(response.Data.ToString());
            }
            return null;
        }

        public virtual void Logout(Referee referee, ITriathlonObserver client)
        {
            SendRequest(new Request(RequestType.LOGOUT, null));
            ReadResponse();
            CloseConnection();
        }

        public virtual List<ParticipantDTO> GetParticipantsByEvent(int idEvent)
        {
            SendRequest(new Request(RequestType.GET_PARTICIPANTS_BY_EVENT, idEvent));
            Response response = ReadResponse();
            return MessagePackSerializer.Deserialize<List<ParticipantDTO>>(MessagePackSerializer.Serialize(response.Data, MessagePack.Resolvers.ContractlessStandardResolver.Options));
        }

        public virtual void AddResult(int idReferee, int idParticipant, int points)
        {
            EventDTO dto = new EventDTO(0, "", idReferee, idParticipant, points);
            SendRequest(new Request(RequestType.ADD_RESULT, dto));
            Response response = ReadResponse();
            if (response.Type == ResponseType.ERROR) throw new Exception(response.Data.ToString());
        }

        public List<EventDTO> GetAllEventsDTO() => null;

        private void InitializeConnection()
        {
            connection = new TcpClient(host, port);
            stream = connection.GetStream();
            finished = false;
            Thread t = new Thread(Run);
            t.Start();
        }

        private void CloseConnection()
        {
            finished = true;
            try {
                stream?.Close();
                connection?.Close();
            } catch (Exception e) { Console.WriteLine(e.Message); }
            _waitHandle.Set(); 
        }

        private void SendRequest(Request request)
        {
            byte[] data = MessagePackSerializer.Serialize(request, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            byte[] length = BitConverter.GetBytes(data.Length);
            stream.Write(length, 0, 4);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }

        private Response ReadResponse()
        {
            bool signaled = _waitHandle.WaitOne(5000); // Timeout de 5 secunde pentru siguranță
            if (!signaled) throw new Exception("Server response timeout");

            lock (responses)
            {
                if (responses.Count > 0)
                    return responses.Dequeue();
                else
                    throw new Exception("No response available in queue");
            }
        }

        private void Run()
        {
            while (!finished)
            {
                try
                {
                    byte[] lengthBuffer = new byte[4];
                    int read = stream.Read(lengthBuffer, 0, 4);
                    if (read < 4) break;

                    int length = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] dataBuffer = new byte[length];
                    int bytesRead = 0;
                    while (bytesRead < length)
                    {
                        bytesRead += stream.Read(dataBuffer, bytesRead, length - bytesRead);
                    }

                    Response response = MessagePackSerializer.Deserialize<Response>(dataBuffer, MessagePack.Resolvers.ContractlessStandardResolver.Options);
                    
                    if (response.Type == ResponseType.UPDATE)
                    {
                        client.UpdateReceived();
                    }
                    else
                    {
                        lock (responses)
                        {
                            responses.Enqueue(response);
                        }
                        _waitHandle.Set();
                    }
                }
                catch (Exception e)
                {
                    if (!finished) Console.WriteLine("Reading error: " + e.Message);
                }
            }
        }
    }
}