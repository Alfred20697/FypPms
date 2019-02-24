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

namespace FypPms.Pages.Supervisor.Supervision
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<Models.Project> Projects { get; set; }
        public IList<Models.Project> CoSuperviseProjects { get; set; }
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
                    Projects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ProjectStatus == "Taken")
                        .Where(p => p.SupervisorId == username)
                        .ToListAsync();

                    CoSuperviseProjects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ProjectStatus == "Taken")
                        .Where(p => p.CoSupervisorId == username)
                        .ToListAsync();

                    Students = await _context.Student.Where(s => s.DateDeleted == null).Where(s => s.ProjectId != null).ToListAsync();

                    foreach (var student in Students)
                    {
                        StudentProjectPairs.Add(student.ProjectId, student.StudentName);
                    }

                    foreach(var project in Projects)
                    {
                        var weeklyLog = await _context.WeeklyLog.Where(w => w.DateDeleted == null).Where(w => w.ProjectId == project.ProjectId).Where(w => w.WeeklyLogStatus == "New" || w.WeeklyLogStatus == "Modified").ToListAsync();

                        var submission = await _context.Submission.Where(s => s.DateDeleted == null).Where(w => w.ProjectId == project.ProjectId).ToListAsync();

                        ProjectLogCountPairs.Add(project.ProjectId, weeklyLog.Count());

                        ProjectSubmissionCountPairs.Add(project.ProjectId, submission.Count());
                    }

                    foreach (var project in CoSuperviseProjects)
                    {
                        var weeklyLog = await _context.WeeklyLog.Where(w => w.DateDeleted == null).Where(w => w.ProjectId == project.ProjectId).Where(w => w.WeeklyLogStatus == "New" || w.WeeklyLogStatus == "Modified").ToListAsync();

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