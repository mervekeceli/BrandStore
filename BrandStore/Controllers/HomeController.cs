using BrandStore.Areas.Identity.Data;
using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private readonly IWebHostEnvironment _hostEnviroment;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnviroment)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _hostEnviroment = hostEnviroment;
        }

        public IActionResult Index()
        {
            var db = _context.Categories.Include(f => f.MainCategory);
            return View(db.ToList());
        }

        public IActionResult Shop()
        {
            var db = _context.Products.Include(f => f.Brand).Include(f => f.Category);
            return View(db.ToList());
        }
		
		public IActionResult ShopSingle(int productId)
        {
            Product _urun = _context.Products.Where(b => b.Id == productId).FirstOrDefault();
            return View(_urun);
        }

        public async Task<IActionResult> CategoriesShop(String name)
        {
            List<Product> _urunler = await _context.Products.Where(b => b.Category.Name == name).ToListAsync();
            return View(_urunler);

        }

        //[HttpPost]
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


        [Authorize]
        public async Task<IActionResult> MyOrder()
        {
            List<Basket> baskets = await _context.Baskets
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == false)
                .ToListAsync();

            return View(baskets);
        }

        [HttpGet]
        public IActionResult CreateBrand()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBrand([Bind("Id,Name,Description,PhotoFile,Active")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                if (brand.PhotoFile == null)
                {
                    return View();
                }
                string imgext;
                imgext = Path.GetExtension(brand.PhotoFile.FileName);


                if (imgext == ".jpg" || imgext == ".png")
                {

                    string path_name = Guid.NewGuid().ToString() + imgext;
                    string saveimg = Path.Combine(_hostEnviroment.WebRootPath, "img", path_name);
                    brand.Photo = path_name;
                    using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                    {
                        await brand.PhotoFile.CopyToAsync(uploadimg);
                    }

                }
                brand.ApplicationUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                brand.Active = true;
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
