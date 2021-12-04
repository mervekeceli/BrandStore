using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrandStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _hostEnviroment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;


        }



        public async Task<IActionResult> Index()
        {
            var ApplicationUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var brand = _context.Brands.Where(b => b.ApplicationUserId == ApplicationUserId).FirstOrDefault();
            var applicationDbContext = _context.Products.Where(b => b.Brand == brand).Include(b => b.Category);
            return View(await applicationDbContext.ToListAsync());
        }
       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Brand)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
      

        [HttpGet]
        public IActionResult Create()
        {
            var ApplicationUserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var brand = _context.Brands.Where(b => b.ApplicationUserId == ApplicationUserId).FirstOrDefault();
            ViewBag.BrandId = brand.Id;
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(a => a.Active == true), "Id", "Name");
            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.MainPhotoFile == null || product.SecondPhotoFile == null || product.ThirdPhotoFile == null)
                {
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View();
                }

                string[] imgext = new string[3];

                imgext[0] = Path.GetExtension(product.MainPhotoFile.FileName);
                imgext[1] = Path.GetExtension(product.SecondPhotoFile.FileName);
                imgext[2] = Path.GetExtension(product.ThirdPhotoFile.FileName);

                if ((imgext[0] == ".jpg" || imgext[0] == ".png") && (imgext[1] == ".jpg" || imgext[1] == ".png") && (imgext[2] == ".jpg" || imgext[2] == ".png"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string path_name = Guid.NewGuid().ToString() + imgext[i];
                        string saveimg = Path.Combine(_hostEnviroment.WebRootPath, "img", path_name);

                        switch (i)
                        {
                            case 0:
                                product.MainPhoto = path_name;
                                using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                                {
                                    await product.MainPhotoFile.CopyToAsync(uploadimg);
                                }
                                break;
                            case 1:
                                product.SecondPhoto = path_name;
                                using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                                {
                                    await product.SecondPhotoFile.CopyToAsync(uploadimg);
                                }
                                break;
                            case 2:
                                product.ThirdPhoto = path_name;
                                using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                                {
                                    await product.ThirdPhotoFile.CopyToAsync(uploadimg);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }


                product.CreateDate = DateTime.Now;
                product.Active = true;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
       
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["BrandId"] = new SelectList(_context.Brands.Where(a => a.Active == true), "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(a => a.Active == true), "Id", "Name", product.CategoryId);
            return View(product);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string[] imgext = new string[3];

                imgext[0] = product.MainPhotoFile == null ? String.Empty : Path.GetExtension(product.MainPhotoFile.FileName);
                imgext[1] = product.SecondPhotoFile == null ? String.Empty : Path.GetExtension(product.SecondPhotoFile.FileName);
                imgext[2] = product.ThirdPhotoFile == null ? String.Empty : Path.GetExtension(product.ThirdPhotoFile.FileName);

                if ((imgext[0] == ".jpg" || imgext[0] == ".png" || imgext[0] == String.Empty) && (imgext[1] == ".jpg" || imgext[1] == ".png" || imgext[1] == String.Empty) && (imgext[2] == ".jpg" || imgext[2] == ".png" || imgext[2] == String.Empty))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (imgext[i] != String.Empty)
                        {
                            string path_name = Guid.NewGuid().ToString() + imgext[i];
                            string saveimg = Path.Combine(_hostEnviroment.WebRootPath, "img", path_name);

                            switch (i)
                            {
                                case 0:
                                    product.MainPhoto = path_name;
                                    using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                                    {
                                        await product.MainPhotoFile.CopyToAsync(uploadimg);
                                    }
                                    break;
                                case 1:
                                    product.SecondPhoto = path_name;
                                    using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                                    {
                                        await product.SecondPhotoFile.CopyToAsync(uploadimg);
                                    }
                                    break;
                                case 2:
                                    product.ThirdPhoto = path_name;
                                    using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                                    {
                                        await product.ThirdPhotoFile.CopyToAsync(uploadimg);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    ViewData["BrandId"] = new SelectList(_context.Brands.Where(a => a.Active == true), "Id", "Name", product.BrandId);
                    ViewData["CategoryId"] = new SelectList(_context.Categories.Where(a => a.Active == true), "Id", "Name", product.CategoryId);
                    return View();
                }

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
      
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Brand)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = _context.Products.Where(x => x.Id == id).FirstOrDefault();

            if (product != null)
            {
                product.Active = !product.Active;
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
       
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        

    }
}
