using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Tanım")]
        public string Description { get; set; }

        [Display(Name = "Fiyat")]
        public int Price { get; set; }

        [Display(Name = "Stok")]
        public int Stock { get; set; }

        [Display(Name = "Renk")]
        public String Color { get; set; }

        [Display(Name = "Cinsiyet")]
        public String Gender { get; set; }

        [Display(Name = "Beden")]
        public String Size { get; set; }

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Marka")]
        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        [Display(Name = "Eklenme Tarihi")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Aktiflik")]
        public bool Active { get; set; }

        [Display(Name = "Ana Fotograf")]
        public string MainPhoto { get; set; }

        [Display(Name = "İkinci Fotograf")]
        public string SecondPhoto { get; set; }

        [Display(Name = "Üçüncü Fotograf")]
        public string ThirdPhoto { get; set; }

        [NotMapped]
        [DisplayName("Ana Fotograf")]
        public IFormFile MainPhotoFile { get; set; }

        [NotMapped]
        [DisplayName("İkinci Fotograf")]
        public IFormFile SecondPhotoFile { get; set; }

        [NotMapped]
        [DisplayName("Üçüncü Fotograf")]
        public IFormFile ThirdPhotoFile { get; set; }
    }
}
