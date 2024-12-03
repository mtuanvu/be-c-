using five_birds_be.Data;
using five_birds_be.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace five_birds_be.Services
{
    public class FooterImageService
    {
        private readonly DataContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public FooterImageService(DataContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // Lấy tất cả ảnh Footer
        public async Task<List<FooterImage>> GetFooterImagesAsync()
        {
            return await _context.FooterImages.ToListAsync();
        }

        // Thêm ảnh Footer mới (và xóa ảnh cũ trên Cloudinary nếu tồn tại)
        public async Task<FooterImage> AddFooterImageAsync(Stream fileStream, string fileName, string altText)
        {
            // Xóa ảnh cũ trên Cloudinary nếu tồn tại
            var existingImages = await _context.FooterImages.ToListAsync();
            foreach (var image in existingImages)
            {
                await _cloudinaryService.DeleteImageAsync(image.ImageUrl);
                _context.FooterImages.Remove(image);
            }

            await _context.SaveChangesAsync();

            // Upload ảnh mới lên Cloudinary
            var imageUrl = await _cloudinaryService.UploadImageAsync(fileStream, fileName);

            // Lưu thông tin ảnh vào database
            var newFooterImage = new FooterImage
            {
                ImageUrl = imageUrl,
                AltText = altText
            };

            _context.FooterImages.Add(newFooterImage);
            await _context.SaveChangesAsync();

            return newFooterImage;
        }

        // Xóa ảnh Footer
        public async Task<bool> DeleteFooterImageAsync(int id)
        {
            var footerImage = await _context.FooterImages.FindAsync(id);
            if (footerImage == null) return false;

            await _cloudinaryService.DeleteImageAsync(footerImage.ImageUrl);
            _context.FooterImages.Remove(footerImage);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
