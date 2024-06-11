namespace KVA.Cinema.Controllers
{
    using KVA.Cinema.Models.Entities;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.IO;

    public class FileController : Controller
    {
        /// <summary>
        /// Maximum size allowed for file in bytes
        /// </summary>
        private const int MAX_FILE_SIZE = 25_000_000; // 25 MB

        private const string POSTER_UPLOAD_PATH = "upload/videoPreview";

        private IWebHostEnvironment HostEnvironment { get; }

        public FileController(IWebHostEnvironment hostEnvironment)
        {
            HostEnvironment = hostEnvironment;
        }

        [HttpPost]
        [ActionName("upload")]
        public FileResponse UploadFile(IFormFile file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Length > MAX_FILE_SIZE)
            {
                throw new ArgumentOutOfRangeException($"File is too big");
            }

            string uploadsFolder = Path.Combine(HostEnvironment.WebRootPath, POSTER_UPLOAD_PATH);

            var result = SaveFile(file, uploadsFolder);

            // Context.Files.Add(new FileEntity { Id = result.Id, ... etc });
            // Context.SaveAsync();

            return new FileResponse
            {
                FileId = result.Id,
                File = GetFileAsBase64(file),
            };
        }

        private FileModel SaveFile(IFormFile file, string destinationFolderName)
        {
            if (file == null || string.IsNullOrWhiteSpace(destinationFolderName))
            {
                throw new ArgumentNullException("Invalid argument");
            }

            DirectoryInfo drInfo = new DirectoryInfo(destinationFolderName);

            if (!drInfo.Exists)
            {
                drInfo.Create();
            }

            Guid fileId = Guid.NewGuid();

            string fileNewName = fileId + Path.GetExtension(file.FileName);
            string pathToFile = Path.Combine(destinationFolderName, fileNewName);

            using (var stream = new FileStream(pathToFile, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return new FileModel
            {
                Id = fileId,
                Name = file.FileName,
                Path = pathToFile,
                Size = file.Length,
                UploadedOn = DateTime.UtcNow,
                //Type = FileType // TODO: from extension
            };
        }

        private string GetFileAsBase64(IFormFile file)
        {
            // todo: get from internet
            throw new Exception("");
        }

        public class FileResponse
        {
            public Guid FileId { get; set; }

            public string File { get; set; }
        }

        public enum FileType
        {
            Image = 1,

            //Video = 2,
        }
    }
}
