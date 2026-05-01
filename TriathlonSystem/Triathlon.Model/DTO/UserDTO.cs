using MessagePack;

namespace Triathlon.Model.DTO
{
    [MessagePackObject]
    public class UserDTO
    {
        [Key(0)] public string Username { get; set; }
        [Key(1)] public string Password { get; set; }

        public UserDTO(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}