namespace five_birds_be.Models
{
    public class FooterImage
    {
        public int Id { get; set; } // ID chính
        public string ImageUrl { get; set; } // URL ảnh
        public string AltText { get; set; } // Văn bản thay thế (tùy chọn)
    }
}
