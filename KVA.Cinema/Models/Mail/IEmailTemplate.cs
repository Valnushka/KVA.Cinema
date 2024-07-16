using System.Collections.Generic;

namespace KVA.Cinema.Models
{
    public interface IEmailTemplate<Tkey, Tvalue>
    {
        Dictionary<Tkey, Tvalue> GetReplacementPairs();
    }
}
