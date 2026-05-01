using System.Collections.Generic;
using Triathlon.Model;

namespace Triathlon.Server.Repository
{
    public interface IRepository<ID, T> where T : Entity
    {
        void Add(T element);
        void Remove(ID id);
        void Update(T element);
        T FindById(ID id);
        IEnumerable<T> GetAll();
    }
}