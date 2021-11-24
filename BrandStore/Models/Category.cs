﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int MainCategoryId { get; set; }

        public MainCategory MainCategory { get; set; }
        
    }
}