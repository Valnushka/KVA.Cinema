namespace KVA.Cinema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class EmailSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
