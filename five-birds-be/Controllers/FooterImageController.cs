using five_birds_be.Models;
using five_birds_be.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/footer-images")]
    public class FooterImageController : ControllerBase
    {
        private readonly FooterImageService _footerImageService;

        public FooterImageController(FooterImageService footerImageService)
        {
            _footerImageService = footerImageService;
        }

        // Lấy tất cả ảnh Footer
        [HttpGet]
        public async Task<IActionResult> GetFooterImages()
        {
            var images = await _footerImageService.GetFooterImagesAsync();
            return Ok(images);
        }

        // Upload ảnh Footer mới
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFooterImage([FromForm] IFormFile file, [FromForm] string altText)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using var fileStream = file.OpenReadStream();
                var footerImage = await _footerImageService.AddFooterImageAsync(fileStream, file.FileName, altText);
                return Ok(footerImage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Xóa ảnh Footer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFooterImage(int id)
        {
            var success = await _footerImageService.DeleteFooterImageAsync(id);
            if (!success) return NotFound("Image not found.");

            return NoContent();
        }
    }
}
