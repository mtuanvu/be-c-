using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace five_birds_be.Models
{
    public enum Role
    {
        ROLE_USER,
        ROLE_ADMIN,
    }

    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Role Role { get; set; } = Role.ROLE_USER;

        public DateTime Create_at { get; set; } = DateTime.Now;

        public DateTime Update_at { get; set;} = DateTime.Now;
    }
}
