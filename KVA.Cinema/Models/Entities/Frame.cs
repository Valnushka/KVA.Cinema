using System;

namespace KVA.Cinema.Entities
{
    public class Frame //добавить свойство для самого кадра
    {
        public Guid Id { get; set; }

        public Guid VideoId { get; set; }

        public virtual Video Video { get; set; }
    }
}
