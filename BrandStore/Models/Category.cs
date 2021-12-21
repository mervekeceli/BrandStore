using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Fotograf")]
        public string Photo { get; set; }

        [NotMapped]
        public IFormFile PhotoFile { get; set; }

        [Display(Name = "Aktiflik")]
        public bool Active { get; set; }

        [Display(Name = "Ana Kategori")]
        public int MainCategoryId { get; set; }

        public MainCategory MainCategory { get; set; }
        
    }
}
