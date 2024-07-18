using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KVA.Cinema.Models.Entities
{
    public interface IEntity
    {
        public Guid Id { get; set; }
    }
}
