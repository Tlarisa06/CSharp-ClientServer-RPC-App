using System.Collections.Generic;
using System.Linq;

namespace Triathlon.Model.DTO
{
    public static class DTOUtils
    {
        // --- Referee ---
        public static RefereeDTO GetDTO(Referee refObj)
        {
            return new RefereeDTO(refObj.Id, refObj.Name, refObj.IdEvent);
        }

        public static Referee GetFromDTO(RefereeDTO refDTO)
        {
            return new Referee(refDTO.Id, refDTO.Name, null, refDTO.IdEvent);
        }

        // --- Participant ---
        public static ParticipantDTO GetDTO(Participant p)
        {
            return new ParticipantDTO(p.Id, p.Name, p.TotalPoints);
        }

        public static List<ParticipantDTO> GetParticipantDTOs(IEnumerable<Participant> participants)
        {
            return participants.Select(p => GetDTO(p)).ToList();
        }

        // --- Event ---
        public static EventDTO GetDTO(Event e)
        {
            return new EventDTO(e.Id, "Proba " + e.Id, e.IdRef, e.IdPart, e.Points);
        }

        public static List<EventDTO> GetEventDTOs(IEnumerable<Event> events)
        {
            return events.Select(e => GetDTO(e)).ToList();
        }
    }
}