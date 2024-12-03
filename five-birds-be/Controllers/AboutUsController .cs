using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using five_birds_be.Models;
using five_birds_be.Data;
using five_birds_be.Services;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/aboutus")]
    public class AboutUsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public AboutUsController(DataContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAboutUs()
        {
            var aboutUsList = await _context.AboutUs.ToListAsync();
            return Ok(aboutUsList);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAboutUs([FromForm] string name, [FromForm] string position, [FromForm] string description, [FromForm] IFormFile? image)
        {
            var aboutUs = new AboutUs
            {
                Name = name,
                Position = position,
                Description = description
            };

            if (image != null)
            {
                using var stream = image.OpenReadStream();
                var imageUrl = await _cloudinaryService.UploadImageAsync(stream, image.FileName);
                aboutUs.ImageUrl = imageUrl;
            }

            _context.AboutUs.Add(aboutUs);
            await _context.SaveChangesAsync();

            return Ok(new { message = "AboutUs added successfully.", aboutUs });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAboutUsById(int id)
        {
            var aboutUs = await _context.AboutUs.FindAsync(id);
            if (aboutUs == null)
            {
                return NotFound(new { message = "AboutUs not found." });
            }
            return Ok(aboutUs);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAboutUs(int id, [FromForm] string name, [FromForm] string position, [FromForm] string description, [FromForm] IFormFile? image)
        {
            var aboutUs = await _context.AboutUs.FindAsync(id);
            if (aboutUs == null)
            {
                return NotFound(new { message = "AboutUs not found." });
            }

            aboutUs.Name = name;
            aboutUs.Position = position;
            aboutUs.Description = description;

            if (image != null)
            {
                using var stream = image.OpenReadStream();
                var imageUrl = await _cloudinaryService.UploadImageAsync(stream, image.FileName);

                if (!string.IsNullOrEmpty(aboutUs.ImageUrl))
                {
                    await _cloudinaryService.DeleteImageAsync(aboutUs.ImageUrl);
                }

                aboutUs.ImageUrl = imageUrl;
            }

            _context.AboutUs.Update(aboutUs);
            await _context.SaveChangesAsync();

            return Ok(new { message = "AboutUs updated successfully.", aboutUs });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAboutUs(int id)
        {
            var aboutUs = await _context.AboutUs.FindAsync(id);
            if (aboutUs == null)
            {
                return NotFound(new { message = "AboutUs not found." });
            }

            if (!string.IsNullOrEmpty(aboutUs.ImageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(aboutUs.ImageUrl);
            }

            _context.AboutUs.Remove(aboutUs);
            await _context.SaveChangesAsync();

            return Ok(new { message = "AboutUs deleted successfully." });
        }
    }
}
