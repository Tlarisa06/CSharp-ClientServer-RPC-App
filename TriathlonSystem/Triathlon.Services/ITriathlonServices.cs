using System.Collections.Generic;
using Triathlon.Model;
using Triathlon.Model.DTO;

namespace Triathlon.Services
{
    public interface ITriathlonServices
    {
        Referee Login(string username, string password, ITriathlonObserver client);
        
        void Logout(Referee referee, ITriathlonObserver client);
        
        List<ParticipantDTO> GetParticipantsByEvent(int idEvent);
        
        void AddResult(int idReferee, int idParticipant, int points);
        
        List<EventDTO> GetAllEventsDTO();
    }
}