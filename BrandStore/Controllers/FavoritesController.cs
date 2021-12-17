using BrandStore.Areas.Identity.Data;
using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()
        {
            Favorite favorite = _context.Favorites
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == true)
                .FirstOrDefault();

            if (favorite == null)
            {
                Favorite newFavorite = new Favorite
                {
                    Status = "YENI",
                    Active = true,
                    ApplicationUser = await _userManager.GetUserAsync(User)
                };

                _context.Add(newFavorite);
                _context.SaveChanges();
            }
            else
            {
                List<FavoriteItem> favoriteItems = await _context.FavoriteItems
                        .Include(x => x.Favorite)
                        .Where(x => x.Favorite.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Favorite.Active == true && x.Active == true)
                        .Include(x => x.Product).ToListAsync();

                if (favoriteItems.Count != 0)
                {
                    ViewData["ToplamFiyat"] = favoriteItems.Sum(x => x.Product.Price);
                    ViewData["BasketID"] = favoriteItems[0].FavoriteId;
                    return View(favoriteItems);
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveFromFavorites(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            Favorite favorite = _context.Favorites
                .Where(x => x.ApplicationUserId == User.FindFirstValue(ClaimTypes.NameIdentifier) && x.Active == true)
                .FirstOrDefault();

            if (favorite != null)
            {
                FavoriteItem favoriteItem = _context.FavoriteItems
                    .Where(x => x.FavoriteId == favorite.Id && x.Active == true && x.ProductId == productId).FirstOrDefault();

                if (favoriteItem != null)
                {
                    favoriteItem.Active = false;
                    _context.Update(favoriteItem);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
