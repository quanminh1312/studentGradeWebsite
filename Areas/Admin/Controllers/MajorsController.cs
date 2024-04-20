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
    public class MajorsController : Controller
    {
        private readonly SIUDBContext _context;

        public MajorsController(SIUDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Majors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Major.ToListAsync());
        }

        // GET: Admin/Majors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var major = await _context.Major
                .FirstOrDefaultAsync(m => m.Id == id);
            if (major == null)
            {
                return NotFound();
            }

            return View(major);
        }

        // GET: Admin/Majors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Majors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Major major)
        {
            if (ModelState.IsValid)
            {
                _context.Add(major);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(major);
        }

        // GET: Admin/Majors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var major = await _context.Major.FindAsync(id);
            if (major == null)
            {
                return NotFound();
            }
            return View(major);
        }

        // POST: Admin/Majors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Major major)
        {
            if (id != major.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(major);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MajorExists(major.Id))
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
            return View(major);
        }

        // GET: Admin/Majors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var major = await _context.Major
                .FirstOrDefaultAsync(m => m.Id == id);
            if (major == null)
            {
                return NotFound();
            }

            return View(major);
        }

        // POST: Admin/Majors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var major = await _context.Major.FindAsync(id);
            if (major != null)
            {
                _context.Major.Remove(major);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MajorExists(int id)
        {
            return _context.Major.Any(e => e.Id == id);
        }
    }
}
