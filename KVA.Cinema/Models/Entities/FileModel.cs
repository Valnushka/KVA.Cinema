using System;
using static KVA.Cinema.Controllers.FileController;

namespace KVA.Cinema.Entities
{
    public class FileModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public FileType Type { get; set; }

        public DateTime UploadedOn { get; set; }
    }
}
