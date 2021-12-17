using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrandStore.Controllers
{
    
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _hostEnviroment;

        public BrandsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _hostEnviroment = hostEnviroment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Brands.Include(b => b.ApplicationUser).ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.Include(b => b.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,PhotoFile,Active")] Brand brand)
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

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }


       
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Active")] Brand brand)
        {

            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string imgext;

                imgext = brand.PhotoFile == null ? String.Empty : Path.GetExtension(brand.PhotoFile.FileName);


                if (imgext == ".jpg" || imgext == ".png" || imgext == String.Empty)
                {

                    if (imgext != String.Empty)
                    {
                        string path_name = Guid.NewGuid().ToString() + imgext;
                        string saveimg = Path.Combine(_hostEnviroment.WebRootPath, "img", path_name);

                        brand.Photo = path_name;
                        using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                        {
                            await brand.PhotoFile.CopyToAsync(uploadimg);
                        }


                    }
                }
                else
                    return View();
                
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
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
            return View(brand);
        }
      
       
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
       
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Brand brand = _context.Brands.Where(x => x.Id == id).FirstOrDefault();

            if (brand != null)
            {
                brand.Active = !brand.Active;
                _context.Update(brand);
                await _context.SaveChangesAsync();

            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
        


    }
}
