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

namespace doancoso.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class SemesGradesStudentsController : Controller
    {
        private readonly SIUDBContext _context;
        private readonly UserManager<User> _userManager;
        public static doancoso.Models.Student StudentLog { get; set; }

        public SemesGradesStudentsController(SIUDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Student/SemesGradesStudents
        public async Task<IActionResult> Index()
        {
            string name = _userManager.GetUserName(HttpContext.User);
            StudentLog = await _context.Student.Include(s => s.Account).Where(s => s.Account.Username == name).FirstOrDefaultAsync();
            var penTrue = _context.SemesGradesStudent.Include(s => s.semester).Include(s => s.Grades).Include(s => s.teacher).Where(s => s.Pending==true && s.StudentId == StudentLog.Id).ToList();
            var penFalse = _context.SemesGradesStudent.Include(s => s.semester).Include(s => s.Grades).Where(s => s.Pending == false && s.StudentId == StudentLog.Id).ToList();
            ViewBag.penTrue = penTrue;
            ViewBag.penFalse = penFalse;
            return View();
        }

        // GET: Student/SemesGradesStudents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semesGradesStudent = await _context.SemesGradesStudent
                .Include(s => s.semester)
                .Include(s => s.student)
                .Include(s => s.teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (semesGradesStudent == null)
            {
                return NotFound();
            }
            semesGradesStudent.Grades = _context.Grade.Where(g => semesGradesStudent.GradeIds.Contains(g.Id)).ToList();
            return View(semesGradesStudent);
        }
        // GET: Student/SemesGradesStudents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semesGradesStudent = await _context.SemesGradesStudent.FindAsync(id);
            if (semesGradesStudent == null)
            {
                return NotFound();
            }
            if (semesGradesStudent.Pending == true)
            {
                return RedirectToAction("Index");
            }
            ViewData["SemesterId"] = new SelectList(_context.Semester, "Id", "Name", semesGradesStudent.SemesterId);
            var grades = _context.Grade.ToList();
            ViewBag.Grades = grades;
            return View(semesGradesStudent);
        }

        // POST: Student/SemesGradesStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GradeIds,GradeNums,TotalGrade,SemesterId")] SemesGradesStudent semesGradesStudent, List<int> gradeIds, List<int> gradeNums)
        {
            semesGradesStudent.GradeIds = new List<int>(gradeIds);
            semesGradesStudent.GradeNums = new List<int>(gradeNums);
            semesGradesStudent.TotalGrade = gradeNums.Sum();
            if (id != semesGradesStudent.Id)
            {
                return NotFound();
            }
            try
            {
                var s = await _context.SemesGradesStudent.FindAsync(id);
                if (s.Pending == true)
                {
                    return RedirectToAction("Index");
                }
                s.Pending = false;
                s.GradeIds = semesGradesStudent.GradeIds;
                s.GradeNums = semesGradesStudent.GradeNums;
                s.TotalGrade = semesGradesStudent.TotalGrade;
                s.SemesterId = semesGradesStudent.SemesterId;
                s.StudentId = StudentLog.Id;
                s.student = StudentLog;
                _context.Update(s);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SemesGradesStudentExists(semesGradesStudent.Id))
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

        public IActionResult Create()
        {
            var trueSemester = _context.SemesGradesStudent.Include(s => s.semester).Where(s => s.StudentId == StudentLog.Id).Select(s => s.semester).ToList();
            var semester = _context.Semester.Where(s => !trueSemester.Any(t => t == s)).ToList();
            ViewData["SemesterId"] = new SelectList(semester, "Id", "Name");
            var grades = _context.Grade.ToList();
            ViewBag.Grades = grades;
            if (semester.Count == 0)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GradeIds,GradeNums,SemesterId")] SemesGradesStudent semesGradesStudent, List<int> gradeIds, List<int> gradeNums)
        {
            semesGradesStudent.StudentId = StudentLog.Id;
            semesGradesStudent.Pending = false;
            semesGradesStudent.GradeIds = new List<int>(gradeIds);
            semesGradesStudent.GradeNums = new List<int>(gradeNums);
            semesGradesStudent.TotalGrade = gradeNums.Sum();
            if (ModelState.IsValid)
            {
                _context.Add(semesGradesStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SemesterId"] = new SelectList(_context.Semester, "Id", "Name", semesGradesStudent.SemesterId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", semesGradesStudent.StudentId);
            ViewData["TeacherId"] = new SelectList(_context.Teacher, "Id", "Name", semesGradesStudent.TeacherId);
            return View(semesGradesStudent);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var semesGradesStudent = await _context.SemesGradesStudent
                .Include(s => s.semester)
                .Include(s => s.student)
                .Include(s => s.teacher)
                .Where(s => s.Pending == false)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (semesGradesStudent == null)
            {
                return NotFound();
            }
            semesGradesStudent.Grades = _context.Grade.Where(g => semesGradesStudent.GradeIds.Contains(g.Id)).ToList();
            return View(semesGradesStudent);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var semesGradesStudent = await _context.SemesGradesStudent.FindAsync(id);
            if (semesGradesStudent == null || semesGradesStudent.Pending == true)
            {
                return RedirectToAction("Index");
            }
            if (semesGradesStudent != null)
            {
                _context.SemesGradesStudent.Remove(semesGradesStudent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SemesGradesStudentExists(int id)
        {
            return _context.SemesGradesStudent.Any(e => e.Id == id);
        }
    }
}
