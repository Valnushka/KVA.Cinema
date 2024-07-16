using System;

namespace KVA.Cinema.Entities
{
    public class Subtitle
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid LanguageId { get; set; }

        public string Text { get; set; }

        public Guid VideoId { get; set; }

        public bool IsDefault { get; set; }

        public virtual Language Language { get; set; }

        public virtual Video Video { get; set; }
    }
}
