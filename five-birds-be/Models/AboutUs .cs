using System.ComponentModel.DataAnnotations;

namespace five_birds_be.Models
{
    public class AboutUs
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
