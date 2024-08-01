using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{
    public static class VoiceSavingHelper
    {
        private static readonly string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "voices");

        static VoiceSavingHelper()
        {
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }

        public static async Task<string> SaveVoiceAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }

            var fileExtension = Path.GetExtension(file.FileName);



            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var uploads = Path.Combine(BasePath, folderName);

            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/voices/{folderName}/{uniqueFileName}";
        }

        public static async Task<List<string>> SaveVoicesAsync(List<IFormFile> files, string folderName)
        {
            List<string> fileNames = new List<string>();

            foreach (var file in files)
            {
                var fileName = await SaveVoiceAsync(file, folderName);
                fileNames.Add(fileName);
            }

            return fileNames;
        }
    }
}
