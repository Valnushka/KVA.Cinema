namespace KVA.Cinema.Models.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using static KVA.Cinema.Controllers.FileController;

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
