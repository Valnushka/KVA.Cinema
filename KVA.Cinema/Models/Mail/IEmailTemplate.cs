using System.Collections.Generic;

namespace KVA.Cinema.Models.Mail
{
    public interface IEmailTemplate<Tkey, Tvalue>
    {
        Dictionary<Tkey, Tvalue> GetReplacementPairs();
    }
}
