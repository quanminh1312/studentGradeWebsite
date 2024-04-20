using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doancoso.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace doancoso.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly SIUDBContext _context;
        private readonly UserManager<User> _userManager;
        public static int AccountId { get; set; }

        public TeacherController(SIUDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Teacher/Teacher
        public async Task<IActionResult> Index()
        {
            string name = _userManager.GetUserName(HttpContext.User);
            AccountId = _context.Accounts.Where(u => u.Username == name).FirstOrDefault()!.Id;
            var teacher = await _context.Teacher.Include(s => s.Major).Include(s => s.Account).Where(s => s.Account.Id == AccountId).FirstOrDefaultAsync();
            if (teacher == null)
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Details",new {id = teacher.Id });
        }

        // GET: Teacher/Teacher/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .Include(t => t.Major)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teacher/Teacher/Create
        public IActionResult Create()
        {
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name");
            return View();
        }

        // POST: Teacher/Teacher/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Degree,MajorId,Phone,Address")] doancoso.Models.Teacher teacher)
        {
            Account account = _context.Accounts.Where(u => u.Id == AccountId).FirstOrDefault()!;
            teacher.Account = account;
            teacher.AccountId = AccountId;
            teacher.Major = _context.Major.Where(c => c.Id == teacher.MajorId).FirstOrDefault()!;
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name", teacher.MajorId);
            return View(teacher);
        }

        // GET: Teacher/Teacher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name", teacher.MajorId);
            return View(teacher);
        }

        // POST: Teacher/Teacher/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Degree,MajorId,Phone,Address")] doancoso.Models.Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var t = _context.Teacher.Where(t => t.Id == teacher.Id).FirstOrDefault();
                    t.Name = teacher.Name;
                    t.Degree = teacher.Degree;
                    t.MajorId = teacher.MajorId;
                    t.Phone = teacher.Phone;
                    t.Address = teacher.Address;
                    _context.Update(t);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            ViewData["MajorId"] = new SelectList(_context.Major, "Id", "Name", teacher.MajorId);
            return View(teacher);
        }
        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }
    }
}
