using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public int Stock { get; set; }

        public String Color { get; set; }

        public String Gender { get; set; }

        public String Size { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Active { get; set; }

        public string MainPhoto { get; set; }

        public string SecondPhoto { get; set; }

        public string ThirdPhoto { get; set; }

        [NotMapped]
        public IFormFile MainPhotoFile { get; set; }

        [NotMapped]
        public IFormFile SecondPhotoFile { get; set; }

        [NotMapped]
        public IFormFile ThirdPhotoFile { get; set; }
    }
}
