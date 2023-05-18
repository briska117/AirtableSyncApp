using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTableIdentity.Models
{
    public class AccountForm
    {

        /// <summary>Gets or sets the email.</summary>
        /// <value>The email.</value>
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>Gets or sets the password.</summary>
        /// <value>The password.</value>
        [Required]
        [MinLength(8)]
        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        /// <summary>Gets or sets the roles.</summary>
        /// <value>The roles.</value>
        [Required]
        public List<Role>? Roles { get; set; } = new List<Role> { Role.Administrator };  
    }
}
