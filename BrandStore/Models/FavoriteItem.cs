using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class FavoriteItem
    {
        public int Id { get; set; }
        public int FavoriteId { get; set; }
        public Favorite Favorite { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public bool Active { get; set; }
    }
}
