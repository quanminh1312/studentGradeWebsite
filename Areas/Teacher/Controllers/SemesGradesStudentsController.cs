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
    public class SemesGradesStudentsController : Controller
    {
        private readonly SIUDBContext _context;
        private readonly UserManager<User> _userManager;
        public static doancoso.Models.Teacher TeacherLog { get; set; }

        public SemesGradesStudentsController(SIUDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Teacher/SemesGradesStudents
        public async Task<IActionResult> Index()
        {
            string name = _userManager.GetUserName(HttpContext.User);
            TeacherLog = await _context.Teacher.Include(s => s.Major).Include(s => s.Account).Where(s => s.Account.Username == name).FirstOrDefaultAsync();
            var penTrue = _context.SemesGradesStudent.Include(s => s.Grades).Include(s => s.semester).Include(s => s.student).ThenInclude(s => s.Class).ThenInclude(s => s.ClassMajor).Include(s => s.teacher).Where(s => s.Pending == true && s.student.Class.ClassMajor == TeacherLog.Major).ToList();
            var penFalse = _context.SemesGradesStudent.Include(s => s.Grades).Include(s => s.semester).Include(s => s.student).ThenInclude(s => s.Class).ThenInclude(s => s.ClassMajor).Include(s => s.teacher).Where(s => s.Pending == false && s.student.Class.ClassMajor == TeacherLog.Major).ToList();
            ViewBag.penTrue = penTrue;
            ViewBag.penFalse = penFalse;
            return View();
        }

        // GET: Teacher/SemesGradesStudents/Details/5
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
        // GET: Teacher/SemesGradesStudents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var penTrue = _context.SemesGradesStudent.Include(s => s.semester).Include(s => s.student).ThenInclude(s => s.Class).ThenInclude(s => s.ClassMajor).Include(s => s.teacher).Where(s => s.Pending == true && s.student.Class.ClassMajor == TeacherLog.Major).ToList();
            var penFalse = _context.SemesGradesStudent.Include(s => s.semester).Include(s => s.student).ThenInclude(s => s.Class).ThenInclude(s => s.ClassMajor).Include(s => s.teacher).Where(s => s.Pending == false && s.student.Class.ClassMajor == TeacherLog.Major).ToList();
            if (id == null)
            {
                return NotFound();
            }
            if (!penFalse.Any(s => s.Id==id) && !penTrue.Any(s => s.Id == id)) return RedirectToAction("Index");
            var semesGradesStudent = await _context.SemesGradesStudent.FindAsync(id);
            if (semesGradesStudent == null)
            {
                return NotFound();
            }
            ViewData["SemesterId"] = new SelectList(_context.Semester, "Id", "Name", semesGradesStudent.SemesterId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Name", semesGradesStudent.StudentId);
            ViewData["TeacherId"] = new SelectList(_context.Teacher, "Id", "Name", semesGradesStudent.TeacherId);
            var grades = _context.Grade.ToList();
            ViewBag.Grades = grades;
            return View(semesGradesStudent);
        }

        // POST: Teacher/SemesGradesStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GradeIds,GradeNums,SemesterId")] SemesGradesStudent semesGradesStudent, List<int> gradeIds, List<int> gradeNums)
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
                s.GradeIds = semesGradesStudent.GradeIds;
                s.GradeNums = semesGradesStudent.GradeNums;
                s.TotalGrade = semesGradesStudent.TotalGrade;
                s.Pending = true;
                s.teacher = TeacherLog;
                s.TeacherId = TeacherLog.Id;
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
        private bool SemesGradesStudentExists(int id)
        {
            return _context.SemesGradesStudent.Any(e => e.Id == id);
        }
    }
}
