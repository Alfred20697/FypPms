using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Supervisor.Moderation
{
    public class ViewWeeklyLogModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewWeeklyLogModel> _logger;
        private readonly IEmailSender _emailSender;

        public Models.Student Student { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public Models.Supervisor CoSupervisor { get; set; }
        public WeeklyLog WeeklyLog { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewWeeklyLogModel(FypPmsContext context, ILogger<ViewWeeklyLogModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
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
                    WeeklyLog = await _context.WeeklyLog
                        .Where(w => w.DateDeleted == null)
                        .Include(w => w.Project)
                        .FirstOrDefaultAsync(w => w.WeeklyLogId == id);

                    if (WeeklyLog == null)
                    {
                        ErrorMessage = "Weekly Log not found";
                        return RedirectToPage("/Supervisor/Moderation/Index");
                    }

                    if (WeeklyLog.Project.ModeratorId != username)
                    {
                        ErrorMessage = "This is not your moderated weekly log!";
                        return RedirectToPage("/Supervisor/Moderation/Index");
                    }

                    Student = await _context.Student
                       .Where(s => s.DateDeleted == null)
                       .FirstOrDefaultAsync(s => s.AssignedId == WeeklyLog.StudentId);

                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == WeeklyLog.SupervisorId);

                    if (WeeklyLog.CoSupervisorId != null)
                    {
                        CoSupervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == WeeklyLog.CoSupervisorId);
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