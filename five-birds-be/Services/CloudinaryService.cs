using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace five_birds_be.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Folder = "aboutus" // Thư mục trên Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString(); // Trả về link ảnh
        }

       public async Task DeleteImageAsync(string imageUrl)
{
    if (string.IsNullOrEmpty(imageUrl))
    {
        throw new ArgumentException("Image URL cannot be null or empty");
    }

    try
    {
        var publicId = ExtractPublicIdFromUrl(imageUrl);
        var deletionParams = new DeletionParams(publicId);
        await _cloudinary.DestroyAsync(deletionParams);
    }
    catch (Exception ex)
    {
        // Log lỗi nếu cần
        Console.WriteLine($"Error deleting image: {ex.Message}");
    }
}


      private string ExtractPublicIdFromUrl(string imageUrl)
{
    if (string.IsNullOrEmpty(imageUrl))
    {
        throw new ArgumentException("Image URL cannot be null or empty");
    }

    var uri = new Uri(imageUrl);
    var segments = uri.AbsolutePath.Split('/');
    var publicIdWithExtension = segments[^1];
    return publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));
}

    }
}
