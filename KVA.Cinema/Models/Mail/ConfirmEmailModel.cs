using System.Collections.Generic;

namespace KVA.Cinema.Models
{
    public class ConfirmEmailModel : IEmailTemplate<string, string>
    {
        public string UserId { get; set; }

        public string Nickname { get; set; }

        public string ConfirmationLink { get; set; }

        public Dictionary<string, string> GetReplacementPairs()
        {
            return new Dictionary<string, string>
            {
                { "{UserId}", UserId },
                { "{Nickname}", Nickname },
                { "{ConfirmationLink}", ConfirmationLink }
            };
        }
    }
}
