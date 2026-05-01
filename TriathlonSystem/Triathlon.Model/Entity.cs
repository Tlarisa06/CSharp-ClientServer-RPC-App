using MessagePack; 

namespace Triathlon.Model
{
    public abstract class Entity
    {
        [IgnoreMember] 
        public int Id { get; set; }

        protected Entity(int id)
        {
            this.Id = id;
        }
    }
}