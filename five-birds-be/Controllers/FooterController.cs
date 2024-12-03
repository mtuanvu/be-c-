using five_birds_be.Models;
using five_birds_be.Services;
using Microsoft.AspNetCore.Mvc;

namespace five_birds_be.Controllers
{
    [ApiController]
    [Route("api/v1/footer")]
    public class FooterController : ControllerBase
    {
        private readonly FooterService _footerService;

        public FooterController(FooterService footerService)
        {
            _footerService = footerService;
        }

        // API: Lấy dữ liệu Footer
        [HttpGet]
        public async Task<IActionResult> GetFooter()
        {
            var footer = await _footerService.GetFooterAsync();
            return Ok(footer);
        }

        // API: Thêm mới hoặc Cập nhật Footer
        [HttpPost("save")]
        public async Task<IActionResult> SaveFooter([FromBody] Footer footer)
        {
            if (footer == null) return BadRequest("Invalid footer data.");

            // Kiểm tra xem Footer đã tồn tại hay chưa
            var existingFooter = await _footerService.GetFooterAsync();
            if (existingFooter == null)
            {
                // Nếu chưa tồn tại, thêm mới
                var newFooter = await _footerService.AddFooterAsync(footer);
                return Ok(newFooter);
            }
            else
            {
                // Nếu đã tồn tại, cập nhật
                existingFooter.Column1 = footer.Column1;
                existingFooter.Column2 = footer.Column2;
                existingFooter.Column3 = footer.Column3;

                var updatedFooter = await _footerService.UpdateFooterAsync(existingFooter);
                return Ok(updatedFooter);
            }
        }
    }
}
