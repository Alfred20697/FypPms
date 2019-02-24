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

namespace FypPms.Pages.Student.Project
{
    public class ProjectDetailsModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ProjectDetailsModel> _logger;
        private readonly IEmailSender _emailSender;

        public Models.Project Project { get; set; }
        public ProjectSpecialization ProjectSpecialization { get; set; }
        public Specialization Specialization { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public Models.Supervisor CoSupervisor { get; set; }
        public IList<Requisition> Requisitions { get; set; }
        public bool CanRequisition { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ProjectDetailsModel(FypPmsContext context, ILogger<ProjectDetailsModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    Project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(m => m.ProjectId == id);

                    if (Project == null)
                    {
                        return NotFound();
                    }

                    ProjectSpecialization = await _context.ProjectSpecialization.FirstOrDefaultAsync(ps => ps.ProjectId == Project.ProjectId);

                    Specialization = await _context.Specialization.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.SpecializationId == ProjectSpecialization.SpecializationId);

                    Supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                    CoSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);

                    Requisitions = await _context.Requisition.Where(s => s.DateDeleted == null).Where(i => i.Sender == username).ToListAsync();
                    
                    if (Requisitions.Count() == 0)
                    {
                        CanRequisition = true;
                    }
                    else
                    {
                        CanRequisition = true;

                        foreach (var Requisition in Requisitions)
                        {
                            if (Requisition.RequisitionStatus != "Rejected")
                            {
                                CanRequisition = false;
                                break;
                            }
                        }
                    }

                    var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).Where(p => p.ProposalStatus == "New").FirstOrDefaultAsync(p => p.Sender == username);

                    if (proposal != null)
                    {
                        CanRequisition = false;
                    }

                    var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

                    if (student.ProjectId != null)
                    {
                        CanRequisition = false;
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var username = HttpContext.Session.GetString("_username");
            var project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == id);
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);

            var Requisition = new Requisition
            {
                ProjectId = id,
                Sender = username,
                Receiver = supervisor.AssignedId,
                RequisitionStatus = "New",
                DateCreated = DateTime.Now
            };

            _context.Requisition.Add(Requisition);
            await _context.SaveChangesAsync();

            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

            var mailsubject = "FYP System - Project Requisition";
            // TODO: Modify the coordinator email and name
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

I would like to requisition for this project:

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

--------------------------------------------------------------------------
Please navigate to Project --> Received Requisition for more information.

Please contact coordinator if there is any questions. Thank you. 
--------------------------------------------------------------------------

Yours Sincerely,
{student.StudentName}
{student.AssignedId}
(FYP Student)";

            await _emailSender.SendEmailAsync(student.StudentEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            // Log Message
            SuccessMessage = $"Requisition for {project.ProjectTitle} sent successfully.";

            return RedirectToPage("/Student/Project/ProjectList");
        }
    }
}