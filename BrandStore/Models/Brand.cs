using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class Brand
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Description { get; set; }
        public bool Active { get; set; }
    }
}
