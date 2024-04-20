using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using doancoso.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace doancoso.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly SIUDBContext _context;
        private readonly UserManager<User> _userManager;
        public static int AccountId { get; set; }

        public StudentController(SIUDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Student/Student
        public async Task<IActionResult> Index()
        {
            string name = _userManager.GetUserName(HttpContext.User);
            AccountId = _context.Accounts.Where(u => u.Username == name).FirstOrDefault()!.Id;
            var student = await _context.Student.Include(s => s.Class).Include(s => s.Account).Where(s => s.Account.Id == AccountId).FirstOrDefaultAsync();
            if (student == null)
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Details", new {id = student.Id });
        }

        // GET: Student/Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Student/Create
        public IActionResult Create()
        {
            ViewData["ClassId"] = new SelectList(_context.Class, "Id", "Name");
            return View();
        }

        // POST: Student/Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Address,ClassId")] doancoso.Models.Student student)
        {
            Account account = _context.Accounts.Where(u => u.Id == AccountId).FirstOrDefault()!;
            student.Account = account;
            student.AccountId = AccountId;
            student.Class = _context.Class.Where(c => c.Id == student.ClassId).FirstOrDefault()!;
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClassId"] = new SelectList(_context.Class, "Id", "Name", student.ClassId);
            return View(student);
        }

        // GET: Student/Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["ClassId"] = new SelectList(_context.Class, "Id", "Name", student.ClassId);
            return View(student);
        }

        // POST: Student/Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Address")] doancoso.Models.Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var s = _context.Student.Where(s => s.Id == student.Id).FirstOrDefault();
                    s.Name = student.Name;
                    s.Phone = student.Phone;
                    s.Address = student.Address;
                    _context.Update(s);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            ViewData["ClassId"] = new SelectList(_context.Class, "Id", "Name", student.ClassId);
            return View(student);
        }
        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
