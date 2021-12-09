using BrandStore.Areas.Identity.Data;
using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrandStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View();
        }
		
		public IActionResult ShopSingle(int productId)
        {
            Product _urun = _context.Products.Where(b => b.Id == productId).FirstOrDefault();
            return View(_urun);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToBasket(int productId)
        {
            var basket = await _context.Baskets
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == true)
                .FirstOrDefaultAsync();

            Product currentProduct = await _context.Products
                .Where(x => x.Id == productId && x.Active == true)
                .FirstOrDefaultAsync();

            if (currentProduct == null) return NotFound();

            if (basket != null)
            {
                BasketItem newBasketItem = new BasketItem
                {
                    BasketId = basket.Id,
                    Product = currentProduct,
                    Active = true
                };

                _context.Add(newBasketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                Basket newBasket = new Basket
                {
                    Status = "YENI",
                    Active = true,
                    ApplicationUser = await _userManager.GetUserAsync(User)
                };

                _context.Add(newBasket);
                _context.SaveChanges();

                BasketItem newBasketItem = new BasketItem
                {
                    BasketId = newBasket.Id,
                    Product = currentProduct,
                    Active = true
                };

                _context.Add(newBasketItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Baskets");
        }


        [HttpGet]
        public IActionResult CreateBrand()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand([Bind("Id,Name,Description,Active")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                brand.ApplicationUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                brand.Active = true;
                var user = await _userManager.FindByIdAsync(brand.ApplicationUserId);
                await _userManager.AddToRoleAsync(user, "Admin");
                _context.Add(brand);
                await _context.SaveChangesAsync();

            }
            return View(brand);
        }


        public async Task<IActionResult> BrandIndex(string brand)
        {
            List<Product> _urunler = await _context.Products.Where(b=>b.Brand.Name==brand).OrderByDescending(x => x.CreateDate).Take(10).ToListAsync();
            return View(_urunler);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
