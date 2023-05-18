using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.Request
{
    internal class MicroserviceError
    {
        public string Message { get; set; }

        public int StatusCode { get; set; }
    }
}
