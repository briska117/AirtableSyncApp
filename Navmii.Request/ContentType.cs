using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.Request
{
    public enum ContentType
    {
        [Description("application/json")]
        Json,
        [Description("application/x-www-form-urlencoded")]
        x_www_form_urlencoded
    }
}
