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

namespace FypPms.Pages.Student.Progress
{
    public class ViewWeeklyLogModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewWeeklyLogModel> _logger;

        public WeeklyLog WeeklyLog { get; set; }
        public Models.Student Student { get; set; }
        public Models.Project Project { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public Models.Supervisor CoSupervisor { get; set; }
        public bool CanEdit { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewWeeklyLogModel(FypPmsContext context, ILogger<ViewWeeklyLogModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    WeeklyLog = await _context.WeeklyLog
                        .Where(w => w.DateDeleted == null)
                        .Include(w => w.Project)
                        .FirstOrDefaultAsync(w => w.WeeklyLogId == id);

                    if (WeeklyLog == null)
                    {
                        return RedirectToPage($"/{usertype}/Progress/Index");
                    }

                    Student = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .Include(s => s.Project)
                        .FirstOrDefaultAsync(s => s.AssignedId == username);

                    if (WeeklyLog.StudentId != Student.AssignedId)
                    {
                        ErrorMessage = "Access Denied! This is not your weekly log!";
                        return RedirectToPage($"/{usertype}/Progress/Index");
                    }

                    Project = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .FirstOrDefaultAsync(p => p.ProjectId == Student.ProjectId);

                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                    if (Project.CoSupervisorId != null)
                    {
                        CoSupervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);
                    }

                    CanEdit = false;

                    if (WeeklyLog.WeeklyLogStatus == "Require Modification")
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