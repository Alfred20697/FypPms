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

namespace FypPms.Pages.Supervisor.Project
{
    public class ViewProjectModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewProjectModel> _logger;

        public Models.Project Project { get; set; }
        public ProjectSpecialization ProjectSpecialization { get; set; }
        public IList<Models.Student> Students { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public Models.Supervisor CoSupervisor { get; set; }
        public bool CanEdit { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewProjectModel(FypPmsContext context, ILogger<ViewProjectModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Project = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Include(p => p.ProjectSpecialization)
                        .FirstOrDefaultAsync(p => p.ProjectId == id);

                    if (Project == null)
                    {
                        return NotFound();
                    }

                    ProjectSpecialization = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .FirstOrDefaultAsync(ps => ps.ProjectId == Project.ProjectId);

                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                    CoSupervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);

                    Students = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Where(s => s.ProjectId == Project.ProjectId)
                        .ToListAsync();

                    CanEdit = false;

                    if (Supervisor.AssignedId == username)
                    {
                        CanEdit = true;
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