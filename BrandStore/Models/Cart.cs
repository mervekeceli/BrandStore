using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class Cart
    {
        
        public int Id { get; set; }

        [ForeignKey("UserDetails")]
        public string UserDetailsId { get; set; }

        public UserDetails UserDetails { get; set; }

        public string Status { get; set; }

        public bool Active { get; set; }
    }
}
