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

namespace FypPms.Pages.Supervisor.Moderation
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<Models.Project> ModeratedProjects { get; set; }
        public IList<Models.Student> Students { get; set; }
        public Dictionary<int?, string> StudentProjectPairs = new Dictionary<int?, string>();
        public Dictionary<int?, int> ProjectLogCountPairs = new Dictionary<int?, int>();
        public Dictionary<int?, int> ProjectSubmissionCountPairs = new Dictionary<int?, int>();
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context, ILogger<IndexModel> logger)
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
                    ModeratedProjects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ModeratorId == username)
                        .ToListAsync();

                    Students = await _context.Student.Where(s => s.DateDeleted == null).Where(s => s.ProjectId != null).ToListAsync();

                    foreach (var student in Students)
                    {
                        StudentProjectPairs.Add(student.ProjectId, student.StudentName);
                    }

                    foreach (var project in ModeratedProjects)
                    {
                        var weeklyLog = await _context.WeeklyLog.Where(w => w.DateDeleted == null).Where(w => w.ProjectId == project.ProjectId).ToListAsync();

                        var submission = await _context.Submission.Where(s => s.DateDeleted == null).Where(w => w.ProjectId == project.ProjectId).ToListAsync();

                        ProjectLogCountPairs.Add(project.ProjectId, weeklyLog.Count());

                        ProjectSubmissionCountPairs.Add(project.ProjectId, submission.Count());
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