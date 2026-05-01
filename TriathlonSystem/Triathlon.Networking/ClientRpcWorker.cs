using System;
using System.Net.Sockets;
using MessagePack;
using Triathlon.Model;
using Triathlon.Model.DTO;
using Triathlon.Services;

namespace Triathlon.Networking
{
    public class ClientRpcWorker : ITriathlonObserver
    {
        private ITriathlonServices server;
        private TcpClient connection;
        private NetworkStream stream;
        private volatile bool connected;

        public ClientRpcWorker(ITriathlonServices server, TcpClient connection)
        {
            this.server = server;
            this.connection = connection;
            try
            {
                stream = connection.GetStream();
                connected = true;
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.StackTrace); 
            }
        }

        public virtual void Run()
        {
            while (connected)
            {
                try
                {
                    byte[] lengthBuffer = new byte[4];
                    int read = stream.Read(lengthBuffer, 0, 4);
                    if (read < 4) throw new Exception("Connection closed");

                    int length = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] dataBuffer = new byte[length];
                    
                    int bytesRead = 0;
                    while (bytesRead < length)
                    {
                        bytesRead += stream.Read(dataBuffer, bytesRead, length - bytesRead);
                    }

                    Request request = MessagePackSerializer.Deserialize<Request>(dataBuffer, MessagePack.Resolvers.ContractlessStandardResolver.Options);
                    
                    Response response = HandleRequest(request);
                    if (response != null)
                    {
                        SendResponse(response);
                    }
                }
                catch (Exception e)
                {
                    connected = false;
                    Console.WriteLine("Worker error: " + e.Message);
                }
            }
            try
            {
                stream.Close();
                connection.Close();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private Response HandleRequest(Request request)
        {
            try
            {
                switch (request.Type)
                {
                    case RequestType.LOGIN:
                        Console.WriteLine("Login request received...");
                        UserDTO udto = MessagePackSerializer.Deserialize<UserDTO>(MessagePackSerializer.Serialize(request.Data, MessagePack.Resolvers.ContractlessStandardResolver.Options));
                        Referee loggedReferee = server.Login(udto.Username, udto.Password, this);
                        return new Response(ResponseType.OK, loggedReferee);

                    case RequestType.LOGOUT:
                        Console.WriteLine("Logout request received...");
                        server.Logout(null, this); 
                        connected = false;
                        return new Response(ResponseType.OK, null);

                    case RequestType.GET_PARTICIPANTS_BY_EVENT:
                        int idEvent = Convert.ToInt32(request.Data);
                        var participants = server.GetParticipantsByEvent(idEvent);
                        return new Response(ResponseType.OK, participants);

                    case RequestType.ADD_RESULT:
                       EventDTO resultDto = MessagePackSerializer.Deserialize<EventDTO>(MessagePackSerializer.Serialize(request.Data, MessagePack.Resolvers.ContractlessStandardResolver.Options));
                        server.AddResult(resultDto.IdReferee, resultDto.IdParticipant, resultDto.Points);
                        return new Response(ResponseType.OK, null);
                }
            }
            catch (Exception e)
            {
                return new Response(ResponseType.ERROR, e.Message);
            }
            return null;
        }
        private void SendResponse(Response response)
        {
            lock (stream)
            {
                byte[] data = MessagePackSerializer.Serialize(response, MessagePack.Resolvers.ContractlessStandardResolver.Options);
                byte[] length = BitConverter.GetBytes(data.Length);
                stream.Write(length, 0, 4);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
        }

        public void UpdateReceived()
        {
            try
            {
                SendResponse(new Response(ResponseType.UPDATE, null));
            }
            catch (Exception e)
            {
                Console.WriteLine("Update error: " + e.Message);
            }
        }
    }
}