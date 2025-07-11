namespace Application.DTOs.Common
{
    public class CreateMediaDto
    {
        public Stream FileStream { get; set; }

        public string FileName { get; set; }

        public string MimeType { get; set; }
    }
}
