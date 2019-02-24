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

namespace FypPms.Pages.Supervisor.Project
{
    public class ViewRequisitionModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewRequisitionModel> _logger;
        private readonly IEmailSender _emailSender;

        public Requisition Requisition { get; set; }
        public Models.Student Student { get; set; }
        public Models.Project Project { get; set; }
        public ProjectSpecialization ProjectSpecialization { get; set; }
        public bool IsRequisition { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewRequisitionModel(FypPmsContext context, ILogger<ViewRequisitionModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    Requisition = await _context.Requisition.Include(i => i.Project).FirstOrDefaultAsync(m => m.RequisitionId == id);

                    if (Requisition == null)
                    {
                        return NotFound();
                    }

                    Student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Requisition.Sender);

                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Requisition.ProjectId);

                    ProjectSpecialization = await _context.ProjectSpecialization.Include(ps => ps.Specialization).FirstOrDefaultAsync(ps => ps.ProjectId == Requisition.ProjectId);
                    
                    IsRequisition = (Project.ProjectStatus == "Available") && (Student.ProjectId == null);

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

        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            // update Requisition status as accepted
            var requisition = await _context.Requisition.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(r => r.RequisitionId == id);

            if (requisition == null)
            {
                ErrorMessage = "Requisition not found";
                return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
            }

            if (requisition.RequisitionStatus != "New")
            {
                ErrorMessage = "Requisition was accepted or rejected. Action denied";
                return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
            }

            requisition.RequisitionStatus = "Accepted";
            requisition.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // update project status as taken
            var project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == requisition.ProjectId);
            project.ProjectStatus = "In Requisition";
            project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == requisition.Sender);

            // check if student had other project. if the student had other then he cannout be confirmed and the requisition status will be set as faiiled
            if (student.ProjectId != null)
            {
                // update requisition as failed
                requisition.RequisitionStatus = "Failed";
                requisition.DateModified = DateTime.Now;
                await _context.SaveChangesAsync();

                var project2 = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == requisition.ProjectId);
                project2.ProjectStatus = "Available";
                project2.DateModified = DateTime.Now;
                await _context.SaveChangesAsync();

                ErrorMessage = "This student had confirmed with other project. You cannot accept the student.";

                return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
            }

            // automate accept student process

            // update requisition as completed
            requisition.RequisitionStatus = "Completed";
            requisition.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // update proposal as completed
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == requisition.ProjectId);
            proposal.ProposalStatus = "Completed";
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // update project as taken
            project.ProjectStatus = "Taken";
            project.ProjectStage = "FYP1";
            project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // confirm student
            student.ProjectId = project.ProjectId;
            student.StudentStatus = "Confirmed";
            student.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // send email
            var username = HttpContext.Session.GetString("_username");
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

            // Email notification
            var mailsubject = "FYP System - Project Accepted";
            var mailbody = $@"Dear {student.StudentName}, 

Your requisition for the following project is accepted and you are confirmed with this project. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please navigate to the system menu under Project --> My Requisition or My Project to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            SuccessMessage = "Requisition confirmed. Student accepted to the project.";

            return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            // update Requisition status as accepted
            var requisition = await _context.Requisition.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(r => r.RequisitionId == id);

            if (requisition == null)
            {
                ErrorMessage = "Requisition not found";
                return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
            }

            if (requisition.RequisitionStatus != "New")
            {
                ErrorMessage = "Requisition was accepted or rejected. Action denied";
                return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
            }

            requisition.RequisitionStatus = "Rejected";
            requisition.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // send email
            var project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == requisition.ProjectId);
            var username = HttpContext.Session.GetString("_username");
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == requisition.Sender);

            // Email notification
            var mailsubject = "FYP System - Requisition Rejected";
            var mailbody = $@"Dear {student.StudentName}, 

Your requisition for the following project is rejected. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please navigate to the system menu under Project --> My Requisition to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            ErrorMessage = "Requisition rejected.";

            return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
        }
    }
}