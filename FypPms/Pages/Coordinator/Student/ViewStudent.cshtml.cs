using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Student
{
    public class ViewStudentModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewStudentModel> _logger;

        public Models.Project Project { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public Models.Supervisor CoSupervisor { get; set; }
        public ProjectSpecialization ProjectSpecialization { get; set; }
        public Models.Student Student { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewStudentModel(FypPmsContext context, ILogger<ViewStudentModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Student = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .FirstOrDefaultAsync(s => s.StudentId == id);

                    if (Student == null)
                    {
                        ErrorMessage = "Student not found";

                        return RedirectToPage("/Coordinator/Student/Index");
                    }

                    Project = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .FirstOrDefaultAsync(p => p.ProjectId == Student.ProjectId);

                    ProjectSpecialization = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .FirstOrDefaultAsync(ps => ps.ProjectId == Student.ProjectId);

                    if(Project != null)
                    {
                        Supervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                        CoSupervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);
                    }
                    
                    return Page();
                }
                else
                {
                    ErrorMessage = "Access Denied";
                    return RedirectToPage($"/{usertype}/Index");
                }
            }
            else
            {
                ErrorMessage = "Login Required";
                return RedirectToPage("/Account/Login");
            }
        }
    }
}