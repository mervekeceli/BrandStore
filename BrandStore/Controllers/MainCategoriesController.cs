using BrandStore.Data;
using BrandStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrandStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MainCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;


        public MainCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.MainCategories.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainCategory = await _context.MainCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mainCategory == null)
            {
                return NotFound();
            }

            return View(mainCategory);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Active")] MainCategory mainCategory)
        {
            if (ModelState.IsValid)
            {
                mainCategory.Active = true;
                _context.Add(mainCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mainCategory);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainCategory = await _context.MainCategories.FindAsync(id);
            if (mainCategory == null)
            {
                return NotFound();
            }
            return View(mainCategory);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Active")] MainCategory mainCategory)
        {

            if (id != mainCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mainCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MainCategoryExists(mainCategory.Id))
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
            return View(mainCategory);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainCategory = await _context.MainCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mainCategory == null)
            {
                return NotFound();
            }

            return View(mainCategory);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            MainCategory mainCategory = _context.MainCategories.Where(x => x.Id == id).FirstOrDefault();

            if (mainCategory != null)
            {
                mainCategory.Active = !mainCategory.Active;
                _context.Update(mainCategory);
                await _context.SaveChangesAsync();

            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool MainCategoryExists(int id)
        {
            return _context.MainCategories.Any(e => e.Id == id);
        }
    }
}
