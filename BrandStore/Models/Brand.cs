using BrandStore.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Display(Name = "Açıklama")]
        public String Description { get; set; }

        [Display(Name = "Fotoğraf")]
        public string Photo { get; set; }

        [NotMapped]
        public IFormFile PhotoFile { get; set; }

        [Display(Name = "Aktiflik")]
        public bool Active { get; set; }

        [Display(Name = "Kullanıcı")]
        public String ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
