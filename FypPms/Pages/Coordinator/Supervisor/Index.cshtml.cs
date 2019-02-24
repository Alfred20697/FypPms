using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Supervisor
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;

        public IList<Models.Supervisor> Supervisors { get; set; }
        public Dictionary<string, int> SupervisorProjectCount { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context, ILogger<IndexModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Supervisors = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .ToListAsync();

                    SupervisorProjectCount = new Dictionary<string, int>();

                    foreach (var supervisor in Supervisors)
                    {
                        var projects = await _context.Project
                            .Where(p => p.DateDeleted == null)
                            .Where(p => p.SupervisorId == supervisor.AssignedId)
                            .Where(p => p.ProjectStatus != "Completed")
                            .ToListAsync();

                        SupervisorProjectCount.Add(supervisor.AssignedId, projects.Count());
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

        public async Task<IActionResult> OnPostInactiveAsync(int id)
        {
            var supervisor = await _context.Supervisor
                .FirstOrDefaultAsync(s => s.SupervisorId == id);

            if (supervisor == null)
            {
                ErrorMessage = "Supervisor not found";
                return RedirectToPage("/Coordinator/Supervisor/Index");
            }

            supervisor.SupervisorStatus = "Inactive";
            supervisor.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // Set all Available project as Not Available
            var projects = await _context.Project
                .Where(p => p.DateDeleted == null)
                .Where(p => p.ProposedBy == "Lecturer")
                .Where(p => p.ProjectStatus == "Available")
                .Where(p => p.SupervisorId == supervisor.AssignedId)
                .ToListAsync();

            foreach (var item in projects)
            {
                item.ProjectStatus = "Not Available";
                item.DateModified = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            await SendEmailAsync(supervisor, projects, "Inactive", "Not Available");

            SuccessMessage = $"Supervisor {supervisor.SupervisorName} status changed to Inactive successfully";

            return RedirectToPage("/Coordinator/Supervisor/Index");
        }

        public async Task<IActionResult> OnPostActiveAsync(int id)
        {
            var supervisor = await _context.Supervisor
                .FirstOrDefaultAsync(s => s.SupervisorId == id);

            if (supervisor == null)
            {
                ErrorMessage = "Supervisor not found";
                return RedirectToPage("/Coordinator/Supervisor/Index");
            }

            supervisor.SupervisorStatus = "Active";
            supervisor.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // Set all Not Available project as Available
            var projects = await _context.Project
                .Where(p => p.DateDeleted == null)
                .Where(p => p.ProposedBy == "Lecturer")
                .Where(p => p.ProjectStatus == "Not Available")
                .Where(p => p.SupervisorId == supervisor.AssignedId)
                .ToListAsync();

            foreach (var item in projects)
            {
                item.ProjectStatus = "Available";
                item.DateModified = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            await SendEmailAsync(supervisor, projects, "Active", "Available");

            SuccessMessage = $"Supervisor {supervisor.SupervisorName} status changed to Active successfully";

            return RedirectToPage("/Coordinator/Supervisor/Index");
        }

        public async Task<IActionResult> OnPostNotCommitteeAsync(int id)
        {
            var supervisor = await _context.Supervisor
                .FirstOrDefaultAsync(s => s.SupervisorId == id);

            if (supervisor == null)
            {
                ErrorMessage = "Supervisor not found";
                return RedirectToPage("/Coordinator/Supervisor/Index");
            }

            supervisor.IsCommittee = false;
            supervisor.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            await SendEmailAsync(supervisor, "Not Committee");

            SuccessMessage = $"Supervisor {supervisor.SupervisorName} unset as FYP committee member successfully";

            return RedirectToPage("/Coordinator/Supervisor/Index");
        }

        public async Task<IActionResult> OnPostCommitteeAsync(int id)
        {
            var supervisor = await _context.Supervisor
                .FirstOrDefaultAsync(s => s.SupervisorId == id);

            if (supervisor == null)
            {
                ErrorMessage = "Supervisor not found";
                return RedirectToPage("/Coordinator/Supervisor/Index");
            }

            supervisor.IsCommittee = true;
            supervisor.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();

            await SendEmailAsync(supervisor, "Committee");

            SuccessMessage = $"Supervisor {supervisor.SupervisorName} set as FYP committee member successfully";

            return RedirectToPage("/Coordinator/Supervisor/Index");
        }

        public async Task SendEmailAsync(Models.Supervisor supervisor, IList<Models.Project> projects, string supervisorStatus, string projectStatus)
        {
            var username = HttpContext.Session.GetString("_username");

            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(c => c.AssignedId == username);

            var projectString = "";

            foreach (var item in projects)
            {
                projectString += $"Project ID: {item.AssignedId} \r\nProject Title: {item.ProjectTitle} \r\n \r\n";
            }

            var mailsubject = "FYP System - Supervisor and Project Status Update";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

Your status in the FYP System had changed to: {supervisorStatus}

These following projects had changed to {projectStatus}.

{projectString}

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);
        }

        public async Task SendEmailAsync(Models.Supervisor supervisor, string supervisorStatus)
        {
            var username = HttpContext.Session.GetString("_username");

            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(c => c.AssignedId == username);

            var mailsubject = "FYP System - Supervisor Status Update";
            var mailbody = "";
            if (supervisorStatus == "Committee")
            {
                mailbody = $@"Dear {supervisor.SupervisorName}, 

You had been assigned as FYP Committee Member.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";
            }
            if (supervisorStatus == "Not Committee")
            {
                mailbody = $@"Dear {supervisor.SupervisorName}, 

You had been removed as FYP Committee Member. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";
            }

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);
        }
    }
}