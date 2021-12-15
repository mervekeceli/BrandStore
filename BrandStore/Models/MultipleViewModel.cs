using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class MultipleViewModel
    {
        public IEnumerable<Brand> BrandViewModel { get; set; }
        public IEnumerable<Category> CategoryViewModel { get; set; }
    }
}
