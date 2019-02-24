using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Moderation
{
    public class AssignModeratorModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<AssignModeratorModel> _logger;
        private readonly IEmailSender _emailSender;

        public Models.Project Project { get; set; }
        public ProjectSpecialization ProjectSpecialization { get; set; }
        public SelectList ModeratorSelectList { get; set; }
        public Dictionary<string, string> SupervisorPairs { get; set; }
        public Dictionary<string, int> SupervisorModerationPairs { get; set; }
        [BindProperty]
        public ModeratorForm Mf { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public AssignModeratorModel(FypPmsContext context, ILogger<AssignModeratorModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class ModeratorForm
        {
            public string Moderator { get; set; }
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

                    ModeratorSelectList = new SelectList(_context.Supervisor.Where(s => s.DateDeleted == null).Where(s => s.AssignedId != Project.SupervisorId || s.AssignedId != Project.CoSupervisorId).OrderBy(s => s.SupervisorName), "AssignedId", "SupervisorName");

                    SupervisorPairs = new Dictionary<string, string>();
                    SupervisorModerationPairs = new Dictionary<string, int>();

                    var supervisors = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .ToListAsync();

                    foreach (var item in supervisors)
                    {
                        SupervisorPairs.Add(item.AssignedId, item.SupervisorName);

                        var projects = await _context.Project.Where(p => p.DateDeleted == null).Where(p => p.ModeratorId == item.AssignedId).ToListAsync();

                        SupervisorModerationPairs.Add(item.AssignedId, projects.Count());
                    }

                    ProjectSpecialization = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .FirstOrDefaultAsync(ps => ps.ProjectId == id);

                    Mf = new ModeratorForm();

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

        public async Task<JsonResult> OnGetCount()
        {
            SupervisorModerationPairs = new Dictionary<string, int>();

            var supervisors = await _context.Supervisor
                .Where(s => s.DateDeleted == null)
                .ToListAsync();

            foreach (var item in supervisors)
            {
                var projects = await _context.Project.Where(p => p.DateDeleted == null).Where(p => p.ModeratorId == item.AssignedId).ToListAsync();

                SupervisorModerationPairs.Add(item.AssignedId, projects.Count());
            }

            return new JsonResult(SupervisorModerationPairs);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (Mf.Moderator == null)
            {
                ErrorMessage = $"Moderator field cannot be empty";

                return RedirectToPage("/Coordinator/Moderation/AssignModerator", id);
            }

            var project = await _context.Project
                .Where(p => p.DateDeleted == null)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            // update project moderator
            project.ModeratorId = Mf.Moderator;
            await _context.SaveChangesAsync();

            var moderator = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Mf.Moderator);

            var username = HttpContext.Session.GetString("_username");
            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(c => c.AssignedId == username);
            
            // send email to moderator
            var mailsubject = "FYP System - Assigned Project to Moderate";
            var mailbody = $@"Dear {moderator.SupervisorName}, 

You have been assigned to moderate the project as detailed below: 

Project Title: {project.ProjectTitle}

Please navigate to the system menu under My Moderation --> Moderation List

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, moderator.SupervisorEmail, mailsubject, mailbody);

            SuccessMessage = $"Moderator assigned for {project.ProjectTitle} successfully.";

            // send email to student
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.ProjectId == project.ProjectId);
            var mailsubject2 = "FYP System - Assigned Moderator";
            var mailbody2 = $@"Dear {student.StudentName}, 

Your project {project.ProjectTitle} had been assigned with the moderator as follows: 

{moderator.SupervisorName}

Please navigate to the system menu under Project --> My Project to check for more information. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, student.StudentEmail, mailsubject2, mailbody2);

            return RedirectToPage("/Coordinator/Moderation/Index");
        }

    }
}