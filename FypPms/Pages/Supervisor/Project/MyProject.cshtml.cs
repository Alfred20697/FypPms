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
    public class MyProjectModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<MyProjectModel> _logger;

        public IList<Models.Project> MyProject { get; set; }
        public IList<Models.Project> TakenProject { get; set; }
        public IList<Models.Project> CoSuperviseProject { get; set; }
        public IList<Models.Supervisor> Supervisors { get; set; }
        public Dictionary<string, string> SupervisorPairs = new Dictionary<string, string>();

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
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    MyProject = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ProjectStatus == "Available")
                        .Where(p => p.SupervisorId == username)
                        .OrderBy(p => p.ProjectStatus)
                        .Include(p => p.ProjectSpecialization)
                        .ToListAsync();

                    TakenProject = await _context.Project
                       .Where(p => p.DateDeleted == null)
                       .Where(p => p.ProjectStatus == "Taken")
                       .Where(p => p.SupervisorId == username)
                       .OrderBy(p => p.ProjectStatus)
                       .Include(p => p.ProjectSpecialization)
                       .ToListAsync();

                    CoSuperviseProject = await _context.Project
                       .Where(p => p.DateDeleted == null)
                       .Where(p => p.ProjectStatus == "Available" || p.ProjectStatus == "Taken")
                       .Where(p => p.CoSupervisorId == username)
                       .OrderByDescending(p => p.ProjectStatus)
                       .Include(p => p.ProjectSpecialization)
                       .ToListAsync();

                    Supervisors = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .ToListAsync();

                    foreach (var supervisor in Supervisors)
                    {
                        SupervisorPairs.Add(supervisor.AssignedId, supervisor.SupervisorName);
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