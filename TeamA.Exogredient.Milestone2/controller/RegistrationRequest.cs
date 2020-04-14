using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace controller
{
    public class RegisrationRequest
    {
        public bool ScopeAnswer { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public string IpAddress { get; set; }

        public string EncryptedPassword { get; set; }

        //public string EncryptedAESKey { get; set; }

        //public string AesIV { get; set; }

        //public int CurrentNumException { get; set; }
    }
}
