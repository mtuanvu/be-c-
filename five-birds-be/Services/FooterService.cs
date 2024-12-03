using five_birds_be.Data;
using five_birds_be.Models;
using Microsoft.EntityFrameworkCore;

namespace five_birds_be.Services
{
    public class FooterService
    {
        private readonly DataContext _context;

        public FooterService(DataContext context)
        {
            _context = context;
        }

        // Lấy Footer đầu tiên từ database
        public async Task<Footer> GetFooterAsync()
        {
            return await _context.Footers.FirstOrDefaultAsync();
        }

        // Thêm mới Footer
        public async Task<Footer> AddFooterAsync(Footer footer)
        {
            _context.Footers.Add(footer);
            await _context.SaveChangesAsync();
            return footer;
        }

        // Cập nhật Footer
        public async Task<Footer> UpdateFooterAsync(Footer footer)
        {
            _context.Footers.Update(footer);
            await _context.SaveChangesAsync();
            return footer;
        }
    }
}
