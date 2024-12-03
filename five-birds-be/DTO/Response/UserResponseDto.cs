using System.ComponentModel.DataAnnotations;

namespace five_birds_be.DTO.Response
{
    public class UserResponseDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime Create_at { get; set; }

        public DateTime Update_at { get; set;}
    }
}
