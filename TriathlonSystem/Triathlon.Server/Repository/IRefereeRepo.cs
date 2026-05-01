using Triathlon.Model;

namespace Triathlon.Server.Repository
{
    public interface IRefereeRepo : IRepository<int, Referee>
    {
        Referee FindByUsernameAndPassword(string username, string password);
    }
}