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

namespace FypPms.Pages.Coordinator.Supervisor
{
    public class ViewSupervisorModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewSupervisorModel> _logger;

        public IList<Models.Project> SuperviseProjects { get; set; }
        public IList<Models.Project> CoSuperviseProjects { get; set; }
        public IList<Models.Project> ModerateProjects { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewSupervisorModel(FypPmsContext context, ILogger<ViewSupervisorModel> logger)
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
                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.SupervisorId == id);

                    if (Supervisor == null)
                    {
                        ErrorMessage = "Supervisor not found";

                        return RedirectToPage("/Coordinator/Supervisor/Index");
                    }

                    SuperviseProjects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.SupervisorId == Supervisor.AssignedId)
                        .Include(p => p.ProjectSpecialization)
                        .ToListAsync();

                    CoSuperviseProjects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.CoSupervisorId == Supervisor.AssignedId)
                        .Include(p => p.ProjectSpecialization)
                        .ToListAsync();

                    ModerateProjects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ModeratorId == Supervisor.AssignedId)
                        .Include(p => p.ProjectSpecialization)
                        .ToListAsync();
                    
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