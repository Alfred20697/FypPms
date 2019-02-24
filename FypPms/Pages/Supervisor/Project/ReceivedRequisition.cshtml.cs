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
    public class ReceivedRequisitionModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ReceivedRequisitionModel> _logger;
        private readonly IEmailSender _emailSender;

        public IList<Requisition> Requisitions { get; set; }
        public int RequisitionCount { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ReceivedRequisitionModel(FypPmsContext context, ILogger<ReceivedRequisitionModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
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
                    Requisitions = await _context.Requisition
                                        .Where(i => i.DateDeleted == null)
                                        .Where(i => i.RequisitionStatus == "New")
                                        .Where(s => s.Receiver == username)
                                        .Include(p => p.Project)
                                        .OrderBy(p => p.RequisitionStatus)
                                        .ToListAsync();

                    RequisitionCount = Requisitions.Count();

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
            var requisition = await _context.Requisition.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(r => r.RequisitionId == id);

            if (requisition == null)
            {
                ErrorMessage = "Requisition not found";
                return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
            }

            if (requisition.RequisitionStatus != "Accepted")
            {
                ErrorMessage = "Requisition was not accepted or rejected. Action denied";
                return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
            }

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
            var project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == requisition.ProjectId);
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

            SuccessMessage = "Project confirmed with student.";

            return RedirectToPage("/Supervisor/Project/ReceivedRequisition");
        }
    }
}