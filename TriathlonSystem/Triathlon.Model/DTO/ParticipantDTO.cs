using MessagePack;

namespace Triathlon.Model.DTO
{
    [MessagePackObject]
    public class ParticipantDTO
    {
        [Key(0)] public int IdParticipant { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public int Points { get; set; } // Trebuie să aibă { get; set; }

        public ParticipantDTO() { } // Constructor gol necesar pentru deserializare

        public ParticipantDTO(int idParticipant, string name, int points)
        {
            this.IdParticipant = idParticipant;
            this.Name = name;
            this.Points = points;
        }
    }
}