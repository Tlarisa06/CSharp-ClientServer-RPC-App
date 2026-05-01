using MessagePack;

namespace Triathlon.Model.DTO
{
    [MessagePackObject]
    public class RefereeDTO
    {
        [Key(0)] public int Id { get; set; }
        [Key(1)] public string Name { get; set; }
        [Key(2)] public int IdEvent { get; set; }

        public RefereeDTO(int id, string name, int idEvent)
        {
            this.Id = id;
            this.Name = name;
            this.IdEvent = idEvent;
        }

        public override string ToString() => $"RefereeDTO{{id={Id}, name='{Name}', idEvent={IdEvent}}}";
    }
}