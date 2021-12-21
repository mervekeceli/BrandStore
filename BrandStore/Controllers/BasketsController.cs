using BrandStore.Areas.Identity.Data;
using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrandStore.Controllers
{
    [Authorize]
    public class BasketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<BasketsController> _localizer;


        public BasketsController(ApplicationDbContext context, IStringLocalizer<BasketsController> localizer, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _localizer = localizer;
        }
       
        /*
         * Bu fonksiyon kullaniciya ait sepeti dondurur. 
         * Eger kullaniciya ait bir sepet daha once olusmamissa olusturur ve dondurur.
         * Eger kullaniciya ait sepet daha once olusmus ve icerisinde eklenmis kitap varsa kitaplarin listesini
         * ve toplam fiyati dondurur.
         */
        public async Task<IActionResult> Index()
        {
            Basket basket = _context.Baskets
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == true)
                .FirstOrDefault();

            if (basket == null)
            {
                Basket newBasket = new Basket
                {
                    Status = "YENI",
                    Active = true,
                    ApplicationUser = await _userManager.GetUserAsync(User)
                };

                _context.Add(newBasket);
                _context.SaveChanges();
            }
            else
            {
                List<BasketItem> basketItems = await _context.BasketItems
                        .Include(x => x.Basket)
                        .Where(x => x.Basket.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Basket.Active == true && x.Active == true)
                        .Include(x => x.Product).ToListAsync();

                if (basketItems.Count != 0)
                {
                    ViewData["ToplamFiyat"] = basketItems.Sum(x => x.Product.Price);
                    ViewData["BasketID"] = basketItems[0].BasketId;
                    return View(basketItems);
                }
            }

            return View();
        }
        
         
        [HttpGet]
        public async Task<IActionResult> RemoveFromBasket(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            Basket basket = _context.Baskets
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == true)
                .FirstOrDefault();

            if (basket != null)
            {
                BasketItem basketItem = _context.BasketItems
                    .Where(x => x.BasketId == basket.Id && x.Active == true && x.ProductId == productId).FirstOrDefault();

                if (basketItem != null)
                {
                    basketItem.Active = false;
                    _context.Update(basketItem);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
      
        /*
         * Kullanici siparisi tamamladiginda bu fonksiyon cagrilir.
         * Kullanicinin sepetindeki urunlerin stoklari dusurulur.
         * Sepet aktifligi false yapilir ve durumu KARGO olarak ayarlanir.
         * Sepet numarasi kullaniciya siparis numarasi olarak geri dondurulur.
         */
        [HttpPost]
        public async Task<IActionResult> BuyProducts(int? basketId)
        {
            Basket basket = await _context.Baskets
                .Where(x => x.Id == basketId)
                .FirstOrDefaultAsync();

            if (basket != null)
            {
                List<BasketItem> basketItem = await _context.BasketItems
                    .Include(x => x.Product)
                    .Where(x => x.BasketId == basketId && x.Active == true)
                    .ToListAsync();

                for (int i = 0; i < basketItem.Count; i++)
                {
                    Product _product = basketItem[i].Product;

                    if (_product.Stock == 0)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            Product tmpProduct = basketItem[i].Product;
                            tmpProduct.Stock++;
                            _context.Update(tmpProduct);
                        }

                        TempData["SiparisMesaj"] = _localizer["SiparisMesaj1"]+ 
                            _product.Name;
                        return RedirectToAction("Index", "Baskets");
                    }

                    _product.Stock = _product.Stock - 1;
                    _context.Update(_product);
                }

                basket.Active = false;
                basket.Status = "KARGO";
                _context.Update(basket);
                await _context.SaveChangesAsync();
            }

            TempData["SiparisMesaj"] = _localizer.GetString("SiparisMesaj2") + basketId;
            return RedirectToAction("Index", "Baskets");
        }
    }
}
