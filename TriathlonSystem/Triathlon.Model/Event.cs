using MessagePack;

namespace Triathlon.Model
{
    [MessagePackObject]
    public class Event : Entity
    {
        [Key(0)]
        public new int Id { get => base.Id; set => base.Id = value; }

        [Key(1)] public int IdRef { get; set; }
        [Key(2)] public int IdPart { get; set; }
        [Key(3)] public int Points { get; set; }

        public Event(int id, int idRef, int idPart, int points) : base(id)
        {
            this.Id = id;
            this.IdRef = idRef;
            this.IdPart = idPart;
            this.Points = points;
        }
    }
}