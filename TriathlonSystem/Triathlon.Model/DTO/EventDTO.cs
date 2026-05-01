using MessagePack;

namespace Triathlon.Model.DTO
{
    [MessagePackObject]
    public class EventDTO
    {
        [Key(0)] public int IdEvent { get; set; }
        [Key(1)] public string NumeProba { get; set; }
        [Key(2)] public int IdReferee { get; set; }
        [Key(3)] public int IdParticipant { get; set; }
        [Key(4)] public int Points { get; set; }

        public EventDTO(int idEvent, string numeProba, int idReferee, int idParticipant, int points)
        {
            this.IdEvent = idEvent;
            this.NumeProba = numeProba;
            this.IdReferee = idReferee;
            this.IdParticipant = idParticipant;
            this.Points = points;
        }
    }
}