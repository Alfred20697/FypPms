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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Project
{
    public class EditProjectModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<EditProjectModel> _logger;
        private readonly IEmailSender _emailSender;

        public Models.Project Project { get; set; }
        public Models.Student Student { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public SelectList SupervisorList { get; set; }
        [BindProperty]
        public EditProjectForm Epf { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public EditProjectModel(FypPmsContext context, ILogger<EditProjectModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class EditProjectForm
        {
            [DisplayName("Change Type")]
            public string Type { get; set; }
            [DisplayName("New Supervisor")]
            public string Supervisor { get; set; }
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

                    if (Project == null)
                    {
                        ErrorMessage = "Project not found";

                        return RedirectToPage("/Coordinator/Project/Index");
                    }

                    Epf = new EditProjectForm();

                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                    SupervisorList = new SelectList(_context.Supervisor.Where(s => s.AssignedId != Project.SupervisorId).Where(s => s.AssignedId != Project.CoSupervisorId), "AssignedId", "SupervisorName");

                    ViewData["Type"] = Enumerable.ToList(new[] { "Edit Supervisor" })
                        .Select(n => new SelectListItem
                        {
                            Value = n.ToString(),
                            Text = n.ToString()
                        }).ToList();

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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (Epf.Type == null)
            {
                ErrorMessage = "Change type cannot be empty.";
                return RedirectToPage("/Coordinator/Project/EditProject", id);
            }

            if (Epf.Type == "Edit Supervisor")
            {
                if (Epf.Supervisor == null)
                {
                    ErrorMessage = "The new supervisor is not selected.";
                    return RedirectToPage("/Coordinator/Project/EditProject", id);
                }

                var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == id);

                if (project == null)
                {
                    ErrorMessage = "Project not found.";
                    return RedirectToPage("/Supervisor/Project/ViewProject", id);
                }

                var oldSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);

                project.SupervisorId = Epf.Supervisor;
                await _context.SaveChangesAsync();

                // propagate changes to all weekly logs
                var weeklyLogs = await _context.WeeklyLog.Where(w => w.ProjectId == id).ToListAsync();
                foreach(var item in weeklyLogs)
                {
                    item.SupervisorId = Epf.Supervisor;
                }
                await _context.SaveChangesAsync();

                await SendEmailAsync(oldSupervisor.AssignedId, project);
                await SendEmailStudentAsync(project);

                SuccessMessage = $"New supervisor assigned for Project {project.AssignedId}. ";
            }

            return RedirectToPage("/Coordinator/Project/Index");
        }

        public async Task SendEmailAsync(string oldSupervisorId, Models.Project project)
        {
            var username = HttpContext.Session.GetString("_username");
            var oldSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == oldSupervisorId);
            var newSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);

            var coordinator = await _context.Coordinator.Where(c => c.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

            var mailsubject = "FYP System - Project (Supervisor Change)";
            var mailbody = $@"Dear {oldSupervisor.SupervisorName} and {newSupervisor.SupervisorName}, 

The following project had changed its supervisor. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Existing Supervisor: 
{oldSupervisor.SupervisorName}

New Supervisor: 
{newSupervisor.SupervisorName}

Please navigate to the system menu under Project --> My Project to check for more information. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, oldSupervisor.SupervisorEmail, mailsubject, mailbody);
            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, newSupervisor.SupervisorEmail, mailsubject, mailbody);
        }

        public async Task SendEmailStudentAsync(Models.Project project)
        {
            var username = HttpContext.Session.GetString("_username");
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.ProjectId == project.ProjectId);
            var coordinator = await _context.Coordinator.Where(c => c.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

            var newSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);

            var mailsubject = "FYP System - Project (Supervisor Change)";
            var mailbody = $@"Dear {student.StudentName}, 

The following project had changed its supervisor. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

New Supervisor: 
{newSupervisor.SupervisorName}

Please navigate to the system menu under Project --> My Project to check for more information. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, student.StudentEmail, mailsubject, mailbody);
        }
    }
}