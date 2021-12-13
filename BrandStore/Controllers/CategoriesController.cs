using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnviroment;
        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment hostEnviroment)
        {
            _context = context;
            _hostEnviroment = hostEnviroment;
        }
     
        
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Categories.Include(b => b.MainCategory);
            return View(await applicationDbContext.ToListAsync());
        }
        

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(b => b.MainCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
       
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["MainCategoryId"] = new SelectList(_context.MainCategories.Where(a => a.Active == true), "Id", "Name");
            return View();
        }
        

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.PhotoFile == null )
                {
                    ViewData["MainCategoryId"] = new SelectList(_context.MainCategories, "Id", "Name", category.MainCategoryId);
                    return View();
                }

                string imgext;

                imgext = Path.GetExtension(category.PhotoFile.FileName);
               

                if (imgext == ".jpg" || imgext == ".png")
                {
                    
                        string path_name = Guid.NewGuid().ToString() + imgext;
                        string saveimg = Path.Combine(_hostEnviroment.WebRootPath, "img", path_name);
                        category.Photo = path_name;
                        using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                        {
                           await category.PhotoFile.CopyToAsync(uploadimg);
                        }
                               
                           
                    
                }

                category.Active = true;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MainCategoryId"] = new SelectList(_context.MainCategories, "Id", "Name", category.MainCategoryId);
            
            return View(category);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["MainCategoryId"] = new SelectList(_context.MainCategories.Where(a => a.Active == true), "Id", "Name", category.MainCategoryId);
           
            return View(category);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                string imgext ;

                imgext = category.PhotoFile == null ? String.Empty : Path.GetExtension(category.PhotoFile.FileName);
                

                if (imgext == ".jpg" || imgext == ".png" || imgext == String.Empty)
                {
                    
                        if (imgext != String.Empty)
                        {
                            string path_name = Guid.NewGuid().ToString() + imgext;
                            string saveimg = Path.Combine(_hostEnviroment.WebRootPath, "img", path_name);

                            category.Photo = path_name;
                            using (var uploadimg = new FileStream(saveimg, FileMode.Create))
                            {
                                 await category.PhotoFile.CopyToAsync(uploadimg);
                            }
                                    
                              
                        }
                }
                else
                {

                    ViewData["MainCategoryId"] = new SelectList(_context.MainCategories.Where(a => a.Active == true), "Id", "Name", category.MainCategoryId);
                    return View();
                }

                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            
            

            ViewData["MainCategoryId"] = new SelectList(_context.MainCategories, "Id", "Name", category.MainCategoryId);
           
            return View(category);
        }
        
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(b => b.MainCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Category category = _context.Categories.Where(x => x.Id == id).FirstOrDefault();

            if (category != null)
            {
                category.Active = !category.Active;
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
       
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
        
    }
}
