using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableIdentity.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public DateTime Expired { get; set; }   
    }
}
