using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doancoso.Models;
using Microsoft.AspNetCore.Authorization;

namespace doancoso.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ClassesController : Controller
    {
        private readonly SIUDBContext _context;

        public ClassesController(SIUDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Classes
        public async Task<IActionResult> Index()
        {
            var sIUDBContext = _context.Class.Include(c => c.ClassMajor);
            return View(await sIUDBContext.ToListAsync());
        }

        // GET: Admin/Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Class
                .Include(c => c.ClassMajor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // GET: Admin/Classes/Create
        public IActionResult Create()
        {
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name");
            return View();
        }

        // POST: Admin/Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,MajorId")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name", @class.MajorId);
            return View(@class);
        }

        // GET: Admin/Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Class.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name", @class.MajorId);
            return View(@class);
        }

        // POST: Admin/Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,MajorId")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
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
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name", @class.MajorId);
            return View(@class);
        }

        // GET: Admin/Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Class
                .Include(c => c.ClassMajor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }

        // POST: Admin/Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @class = await _context.Class.FindAsync(id);
            if (@class != null)
            {
                _context.Class.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassExists(int id)
        {
            return _context.Class.Any(e => e.Id == id);
        }
    }
}
