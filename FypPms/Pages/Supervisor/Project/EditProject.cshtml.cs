using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

namespace FypPms.Pages.Supervisor.Project
{
    public class EditProjectModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<EditProjectModel> _logger;
        private readonly IEmailSender _emailSender;

        //[BindProperty]
        //public Models.Student Student { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        //[BindProperty]
        //public Models.Supervisor Supervisor { get; set; }
        //[BindProperty]
        //public Models.Supervisor CoSupervisor { get; set; }
        [BindProperty]
        public List<ProjectSpecialization> ProjectSpecializations { get; set; }
        [BindProperty]
        public EditProjectForm Epf { get; set; }
        public bool CanCancel { get; set; }

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
            [DisplayName("Project Title")]
            public string Title { get; set; }
            [DisplayName("Project Description")]
            public string Description { get; set; }
            [DisplayName("Project Objective")]
            public string Objective { get; set; }
            [DisplayName("Project Scope")]
            public string Scope { get; set; }
            [DisplayName("Reason to Change")]
            public string Reason { get; set; }
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
                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == id && p.SupervisorId == username);

                    if (Project == null)
                    {
                        ErrorMessage = "Project not found.";
                        return RedirectToPage("/Supervisor/Project/ViewProject", id);
                    }

                    if (Project.ProjectStatus != "Available" && Project.ProjectStatus != "Taken")
                    {
                        ErrorMessage = "Action denied.";
                        return RedirectToPage("/Supervisor/Project/ViewProject", id);
                    }

                    CanCancel = Project.ProjectStatus == "Available" ? true : false;

                    Epf = new EditProjectForm();

                    //Supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                    //CoSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);

                    ProjectSpecializations = await _context.ProjectSpecialization.Include(p => p.Specialization).Where(p => p.ProjectId == Project.ProjectId).ToListAsync();

                    Epf.Title = Project.ProjectTitle;
                    Epf.Description = ProjectSpecializations[0].ProjectDescription;
                    Epf.Objective = ProjectSpecializations[0].ProjectObjective;
                    Epf.Scope = ProjectSpecializations[0].ProjectScope;

                    ViewData["Type"] = Enumerable.ToList(new[] { "Edit Project" })
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
            //if (Epf.Type == "Cancel Project")
            //{
            //    if (Epf.Reason == null)
            //    {
            //        ErrorMessage = "The reason cannot be empty.";
            //        return RedirectToPage("/Supervisor/Project/EditProject", id);
            //    }

            //    var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == id);

            //    if (project == null)
            //    {
            //        ErrorMessage = "Project not found.";
            //        return RedirectToPage("/Supervisor/Project/ViewProject", id);
            //    }

            //    if (project.ProjectStatus != "Available" && project.ProjectStatus != "Taken")
            //    {
            //        ErrorMessage = "Action denied";
            //        return RedirectToPage("/Supervisor/Project/ViewProject", id);
            //    }

            //    var changeRequest = new ChangeRequest
            //    {
            //        ChangeRequestType = Epf.Type,
            //        ReasonToChange = Epf.Reason,
            //        ChangeRequestStatus = "New",
            //        DateCreated = DateTime.Now
            //    };


            //}

            if (Epf.Type == null)
            {
                ErrorMessage = "Change type cannot be empty.";
                return RedirectToPage("/Supervisor/Project/EditProject", id);
            }

            if (Epf.Type == "Edit Project")
            {
                if (Epf.Reason == null)
                {
                    ErrorMessage = "The reason cannot be empty.";
                    return RedirectToPage("/Supervisor/Project/EditProject", id);
                }

                if (Epf.Title == null || Epf.Description == null || Epf.Objective == null || Epf.Scope == null)
                {
                    ErrorMessage = "The field cannot be empty.";
                    return RedirectToPage("/Supervisor/Project/EditProject", id);
                }

                var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == id);

                if (project == null)
                {
                    ErrorMessage = "Project not found.";
                    return RedirectToPage("/Supervisor/Project/ViewProject", id);
                }

                if (project.ProjectStatus != "Available" && project.ProjectStatus != "Taken")
                {
                    ErrorMessage = "Action denied";
                    return RedirectToPage("/Supervisor/Project/ViewProject", id);
                }

                var changeRequest = new ChangeRequest
                {
                    ChangeRequestType = Epf.Type,
                    Title = Epf.Title,
                    Description = Epf.Description,
                    Objective = Epf.Objective,
                    Scope = Epf.Scope,
                    ReasonToChange = Epf.Reason,
                    ChangeRequestStatus = "New",
                    ProjectId = id,
                    DateCreated = DateTime.Now
                };
                _context.ChangeRequest.Add(changeRequest);
                await _context.SaveChangesAsync();

                var username = HttpContext.Session.GetString("_username");
                var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

                var coordinator = await _context.Coordinator.Where(c => c.DateDeleted == null).FirstOrDefaultAsync();

                var mailsubject = "FYP System - Project (Request for Modification)";
                var mailbody = $@"Dear {coordinator.CoordinatorName}, 

The following project is requesting for change approval. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

New Project Title: 
{Epf.Title}

New Project Description:
{Epf.Description}

New Project Objective:
{Epf.Objective}

New Project Scope:
{Epf.Scope}

Please navigate to the system menu under Project --> Manage Project Change to check for more information. 

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

                await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, coordinator.CoordinatorEmail, mailsubject, mailbody);

                //var projectSpecializations = await _context.ProjectSpecialization.Include(p => p.Specialization).Where(ps => ps.ProjectId == project.ProjectId).ToListAsync();

                //projectSpecializations[0].ProjectDescription = Epf.Description;
                //projectSpecializations[0].ProjectObjective = Epf.Objective;
                //projectSpecializations[0].ProjectScope = Epf.Scope;
                //await _context.SaveChangesAsync();

                //                var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);

                //                var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.ProjectId == project.ProjectId);

                //                if (student != null)
                //                {
                //                    var mailsubject = "FYP System - Project (Modified)";
                //                    var mailbody = $@"Dear {student.StudentName}, 

                //Your following project had been modified.

                //Project ID: {project.AssignedId}
                //Project Title: {project.ProjectTitle}

                //New Project Description:
                //{Epf.Description}

                //New Project Objective:
                //{Epf.Objective}

                //New Project Scope:
                //{Epf.Scope}

                //Please navigate to the system menu under Project --> My Project to check for more information. 

                //Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

                //Yours Sincerely,
                //{supervisor.SupervisorName}
                //(FYP Supervisor)";

                //                    await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);
                //                }

                SuccessMessage = "Change request sent.";
            }

            return RedirectToPage("/Supervisor/Project/ViewProject", id);
        }

        public IActionResult OnPostCancel(int id)
        {
            return RedirectToPage("/Supervisor/Project/ViewProject", id);
        }
    }
}