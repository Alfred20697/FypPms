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

namespace FypPms.Pages.Student.Project
{
    public class MyProjectModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<MyProjectModel> _logger;

        [BindProperty]
        public Models.Student Student { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public Models.Supervisor Supervisor { get; set; }
        [BindProperty]
        public Models.Supervisor CoSupervisor { get; set; }
        public Models.Supervisor Moderator { get; set; }
        [BindProperty]
        public List<ProjectSpecialization> ProjectSpecializations { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public MyProjectModel(FypPmsContext context, ILogger<MyProjectModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Student.ProjectId);

                    if (Project != null)
                    {
                        Supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);
                        CoSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);
                        Moderator = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.ModeratorId);

                        ProjectSpecializations = await _context.ProjectSpecialization.Include(p => p.Specialization).Where(p => p.ProjectId == Project.ProjectId).ToListAsync();
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