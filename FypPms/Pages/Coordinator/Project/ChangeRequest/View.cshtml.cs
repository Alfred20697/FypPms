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

namespace FypPms.Pages.Coordinator.Project.ChangeRequest
{
    public class ViewModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewModel> _logger;
        private readonly IEmailSender _emailSender;

        public Models.ChangeRequest ChangeRequest { get; set; }
        public Models.Project Project { get; set; }
        public Models.ProjectSpecialization ProjectSpecialization { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewModel(FypPmsContext context, ILogger<ViewModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
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
                    ChangeRequest = await _context.ChangeRequest
                        .Where(cr => cr.DateDeleted == null)
                        .FirstOrDefaultAsync(cr => cr.ChangeRequestId == id);

                    if (ChangeRequest == null)
                    {
                        return RedirectToPage("Coordinator/Project/ChangeRequest/Index");
                    }

                    Project = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .FirstOrDefaultAsync(p => p.ProjectId == ChangeRequest.ProjectId);

                    ProjectSpecialization = await _context.ProjectSpecialization
                        .FirstOrDefaultAsync(ps => ps.ProjectId == Project.ProjectId);

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

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var changeRequest = await _context.ChangeRequest
                .Include(c => c.Project)
                .FirstOrDefaultAsync(c => c.ChangeRequestId == id);

            if (changeRequest == null)
            {
                ErrorMessage = "Change request not found.";
                return RedirectToPage("/Coordinator/Project/ChangeRequest/Index");
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(p => p.ProjectId == changeRequest.ProjectId);

            var projectSpecialization = await _context.ProjectSpecialization
                .FirstOrDefaultAsync(ps => ps.ProjectId == project.ProjectId);

            changeRequest.ChangeRequestStatus = "Approved";
            await _context.SaveChangesAsync();

            project.ProjectTitle = changeRequest.Title;
            await _context.SaveChangesAsync();

            projectSpecialization.ProjectDescription = changeRequest.Description;
            projectSpecialization.ProjectObjective = changeRequest.Objective;
            projectSpecialization.ProjectScope = changeRequest.Scope;
            await _context.SaveChangesAsync();

            // send email
            var username = HttpContext.Session.GetString("_username");
            var coordinator = await _context.Coordinator.FirstOrDefaultAsync(c => c.AssignedId == username);
            var supervisor = await _context.Supervisor.FirstOrDefaultAsync(s => s.AssignedId == changeRequest.Project.SupervisorId);

            var mailsubject = "FYP System - Project Change Request (Approved)";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

The change request for the following project had been approved. 

Project ID: {changeRequest.Project.AssignedId}
Project Title: {changeRequest.Project.ProjectTitle}

New Project Title: 
{changeRequest.Title}

New Project Description:
{changeRequest.Description}

New Project Objective:
{changeRequest.Objective}

New Project Scope:
{changeRequest.Scope}

Please navigate to the system menu under Project --> My Project to check for more information. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            // send email to student if exists
            var student = await _context.Student.FirstOrDefaultAsync(s => s.ProjectId == project.ProjectId);

            if (student != null)
            {
                var mailsubject2 = "FYP System - Project Change (Approved)";
                var mailbody2 = $@"Dear {student.StudentName}, 

There are some changes to the following project: 

Project ID: {changeRequest.Project.AssignedId}
Project Title: {changeRequest.Project.ProjectTitle}

New Project Title: 
{changeRequest.Title}

New Project Description:
{changeRequest.Description}

New Project Objective:
{changeRequest.Objective}

New Project Scope:
{changeRequest.Scope}

Please navigate to the system menu under Project --> My Project to check for more information. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

                await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, supervisor.SupervisorEmail, mailsubject2, mailbody2);
            }


            SuccessMessage = "Change request approved";

            return RedirectToPage("/Coordinator/Project/ChangeRequest/Index");
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var changeRequest = await _context.ChangeRequest
                .Include(c => c.Project)
                .FirstOrDefaultAsync(c => c.ChangeRequestId == id);

            if (changeRequest == null)
            {
                ErrorMessage = "Change request not found.";
                return RedirectToPage("/Coordinator/Project/ChangeRequest/Index");
            }

            changeRequest.ChangeRequestStatus = "Rejected";
            await _context.SaveChangesAsync();

            // send email
            var username = HttpContext.Session.GetString("_username");
            var coordinator = await _context.Coordinator.FirstOrDefaultAsync(c => c.AssignedId == username);
            var supervisor = await _context.Supervisor.FirstOrDefaultAsync(s => s.AssignedId == changeRequest.Project.SupervisorId);

            var mailsubject = "FYP System - Project Change Request (Rejected)";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

The change request for the following project had been rejected. 

Project ID: {changeRequest.Project.AssignedId}
Project Title: {changeRequest.Project.ProjectTitle}

Please navigate to the system menu under Project --> My Project to check for more information. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            ErrorMessage = "Change request rejected";

            return RedirectToPage("/Coordinator/Project/ChangeRequest/Index");
        }
    }
}