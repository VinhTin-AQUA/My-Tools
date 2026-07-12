namespace Thanh_Video_Labeler.Services
{
    public class FileService
    {
        private readonly IWebHostEnvironment env;

        public FileService(IWebHostEnvironment env)
        {
            this.env = env;
        }
        
        public async Task<string> SaveFile(IFormFile file, string uploadPath)
        {
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var filePath = Path.Combine(uploadPath, file.FileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return filePath;
        }
    }
}