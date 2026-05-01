using System.Collections.Generic;
using Triathlon.Model;

namespace Triathlon.Server.Repository
{
    public interface IParticipantRepo : IRepository<int, Participant>
    {
        IEnumerable<Participant> FindAllSortedByPoints();
    }
}