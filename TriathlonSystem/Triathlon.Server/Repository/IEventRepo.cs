using System.Collections.Generic;
using Triathlon.Model;

namespace Triathlon.Server.Repository
{
    public interface IEventRepo : IRepository<int, Event>
    {
        IEnumerable<Event> FindByRefereeId(int idReferee);
        Event FindByRefereeAndParticipant(int idReferee, int idParticipant);
        int GetTotalPointsForParticipant(int idParticipant);
        void RemoveAllByRefereeAndParticipant(int idReferee, int idParticipant);
    }
}