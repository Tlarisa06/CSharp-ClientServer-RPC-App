using MessagePack;

namespace Triathlon.Model
{
    [MessagePackObject]
    public class Referee : Entity
    {
        [Key(0)] // Id-ul devine cheia 0 aici
        public new int Id { get => base.Id; set => base.Id = value; }

        [Key(1)] public string Name { get; set; }
        [Key(2)] public string Password { get; set; }
        [Key(3)] public int IdEvent { get; set; }

        public Referee(int id, string name, string password, int idEvent) : base(id)
        {
            this.Name = name;
            this.Password = password;
            this.IdEvent = idEvent;
        }
    }
}