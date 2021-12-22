using BrandStore.Areas.Identity.Data;
using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer<HomeController> _localizer;
        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer,ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostEnviroment)
        {
            _logger = logger;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _hostEnviroment = hostEnviroment;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            var brands = _context.Brands;
            var categories = _context.Categories;
            var products = _context.Products;

            MultipleViewModel multipleViewModel = new MultipleViewModel();
            multipleViewModel.BrandViewModel = brands;
            multipleViewModel.CategoryViewModel = categories;
            multipleViewModel.ProductViewModel = products;

            return View(multipleViewModel);
        }

        public IActionResult Shop(string? gender, string? brand, string? color, string? size, string? category, string? price)
        {
            //List<Product> _products = _context.Products.Include(f => f.Brand).Include(f => f.Category).GroupBy(p=>p.Name).Select(x=>x.First()).ToList();
            List<Product> _products = _context.Products.Where(f => f.Active == true).Include(f => f.Brand).Include(f => f.Category).Select(x => x.Name).Distinct() // (1)
                .SelectMany(key => _context.Products.Include(f => f.Brand).Include(f => f.Category).Where(x => x.Name == key).Take(1)) // (2)
                .ToList();
            if (!string.IsNullOrEmpty(gender))
            {
                _products = _products.Where(x => x.Gender == gender).ToList();
            }
            if (!string.IsNullOrEmpty(color))
            {
                _products = _products.Where(x => x.Color == color).ToList();
            }
            if (!string.IsNullOrEmpty(size))
            {
                _products = _products.Where(x => x.Size == size).ToList();
            }
            if (!string.IsNullOrEmpty(brand))//HATA
            {
                _products = _products.Where(x => x.Brand.Name == brand).ToList();
            }
            if (!string.IsNullOrEmpty(category))//HATA
            {
                //var categoryId = _context.Categories.Where(y => y.Name == category).Select(y => y.Id).FirstOrDefault();
                _products = _products.Where(x => x.Category.Name == category).ToList();
            }
            if (!string.IsNullOrEmpty(price))
            {
                string[] prices = price.Split("-");
                int lowPrice = Int32.Parse(prices[0]);
                int highPrice = Int32.Parse(prices[1]);

                _products = _products.Where(x => x.Price >= lowPrice && x.Price <= highPrice).ToList();
            }
            List<string> _bedenler = new List<string>();
            List<string> _renkler = new List<string>();
            foreach (var item in _products)
            {
                _bedenler.Add(item.Size);
                _renkler.Add(item.Color);
            }
            _bedenler = _bedenler.Distinct().ToList();
            _renkler = _renkler.Distinct().ToList();

            ViewBag.Bedenler = _bedenler;
            ViewBag.Renkler = _renkler;

            return View(_products);
        }

        public async Task<IActionResult> ShopSingle(string productName, int category)
        {
            List<Product> products = await _context.Products.Include(b => b.Brand).Where(b => b.CategoryId == category).Take(3).ToListAsync();
            Product product = _context.Products.Include(b=>b.Brand).Where(b => b.Name == productName).FirstOrDefault();
            List<Product> _products = _context.Products.Include(b => b.Brand).Where(b => b.Name == productName).ToList();
            List<string> _bedenler = new List<string>();
            List<string> _newbedenler = new List<string>();
            foreach (var item in _products)
            {
                _bedenler.Add(item.Size);
            }

            _newbedenler = _bedenler.Distinct().ToList();
            ViewBag.Bedenler = _newbedenler;

            products.Add(product);

            return View(products);


        }
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

        public async Task<IActionResult> AddProductToFavorites(int productId)
        {
            var favorite = await _context.Favorites
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == true)
                .FirstOrDefaultAsync();

            Product currentProduct = await _context.Products
                .Where(x => x.Id == productId && x.Active == true)
                .FirstOrDefaultAsync();

            if (currentProduct == null) return NotFound();

            if (favorite != null)
            {
                FavoriteItem newFavorteItem = new FavoriteItem
                {
                    FavoriteId = favorite.Id,
                    Product = currentProduct,
                    Active = true
                };

                _context.Add(newFavorteItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                Favorite newFavorite = new Favorite
                {
                    Status = "YENI",
                    Active = true,
                    ApplicationUser = await _userManager.GetUserAsync(User)
                };

                _context.Add(newFavorite);
                _context.SaveChanges();

                FavoriteItem newFavoriteItem = new FavoriteItem
                {
                    FavoriteId = favorite.Id,
                    Product = currentProduct,
                    Active = true
                };

                _context.Add(newFavoriteItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Favorites");
        }

        public async Task<IActionResult> SearchProducts(string searchItem)
        {
            List<Product> products = await _context.Products.Where(x => x.Name.ToLower().Contains(searchItem.ToLower())).ToListAsync();
            return View(products);
        }


        [Authorize]
        public async Task<IActionResult> MyOrder()
        {
            List<Basket> baskets = await _context.Baskets
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == false)
                .ToListAsync();

            return View(baskets);
        }

        public async Task<IActionResult> OrderDetails(int? orderId)
        {
            if (orderId == null)
            {
                return NotFound();
            }

            List<BasketItem> basketItems = await _context.BasketItems
                .Where(b => b.BasketId == orderId && b.Active == true)
                .Include(b => b.Product)
                .Include(b => b.Product.Brand)
                .Include(b => b.Product.Category)
                .ToListAsync();

            return View(basketItems);
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
               // brand.Active = true;
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [Route("Home/ErrorPage/{statusCode}")]
        public IActionResult ErrorPage(int statusCode)
        {
            switch (statusCode)
            {
                case 901:
                    ViewBag.ErrorMessage = _localizer["ErrorMessage1"];
                    break;
                default:
                    ViewBag.ErrorMessage = _localizer["ErrorMessage2"];
                    break;
            }
            return View();
        }
        public IActionResult ErrorPage(string ReturnUrl)
        {
            ViewBag.ErrorMessage = _localizer["ErrorMessage2"] + ": " + ReturnUrl;
            return View();
        }
        [HttpPost]
        public IActionResult LangSetting(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) }
                );
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
