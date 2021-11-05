using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class UserDetails: IdentityUser
    {
        public string Name { get; set; }

        public string Lastname { get; set; }
    }
}
