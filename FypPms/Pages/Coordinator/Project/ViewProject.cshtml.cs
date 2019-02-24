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

namespace FypPms.Pages.Coordinator.Project
{
    public class ViewProjectModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewProjectModel> _logger;

        public Models.Project Project { get; set; }
        public ProjectSpecialization ProjectSpecialization { get; set; }
        public Dictionary<string, string> SupervisorPairs { get; set; }
        public Models.Student Student { get; set; }
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
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Project = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .FirstOrDefaultAsync(p => p.ProjectId == id);

                    if(Project == null)
                    {
                        ErrorMessage = "Project not found";

                        return RedirectToPage("/Coordinator/Project/Index");
                    }

                    ProjectSpecialization = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .FirstOrDefaultAsync(ps => ps.ProjectId == id);

                    var supervisors = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .ToListAsync();

                    SupervisorPairs = new Dictionary<string, string>();

                    foreach(var item in supervisors)
                    {
                        SupervisorPairs.Add(item.AssignedId, item.SupervisorName);
                    }

                    Student = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.ProjectId == id);

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