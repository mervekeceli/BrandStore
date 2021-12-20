using BrandStore.Areas.Identity.Data;
using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrandStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            int[] totalArray = new int[2];
            totalArray= TotalAlisveris();
            ViewBag.TotalFiyat = totalArray[0];
            ViewBag.TotalAlisveris = totalArray[1];
            return View();
        }
        public int[] TotalAlisveris()
        {
            int[] totalArray=new int[2];
            var ApplicationUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Brand> _brands = _context.Brands.Include(b => b.ApplicationUser).Where(b => b.Active == false).ToList();
            
            List<BasketItem> _basketitems = _context.BasketItems.Where(p => p.Product.Brand.ApplicationUserId == ApplicationUserId).ToList();
            totalArray[0]= _basketitems.Sum(x => x.Product.Price);
            totalArray[1] = _basketitems.Count();
            return totalArray; 
        }
    }
}
