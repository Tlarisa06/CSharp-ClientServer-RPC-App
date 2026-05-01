using MessagePack;

namespace Triathlon.Model
{
    [MessagePackObject]
    public class Participant : Entity
    {
        [Key(0)]
        public new int Id { get => base.Id; set => base.Id = value; }

        [Key(1)] public string Name { get; set; }
        [Key(2)] public int TotalPoints { get; set; }

        public Participant(int id, string name, int totalPoints) : base(id)
        {
            this.Id = id;
            this.Name = name;
            this.TotalPoints = totalPoints;
        }
    }
}