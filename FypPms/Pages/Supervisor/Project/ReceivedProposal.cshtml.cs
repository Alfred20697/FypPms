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
    public class ReceivedProposalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ReceivedProposalModel> _logger;
        private readonly IEmailSender _emailSender;

        public IList<Proposal> Proposals { get; set; }
        public int ProposalCount { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ReceivedProposalModel(FypPmsContext context, ILogger<ReceivedProposalModel> logger, IEmailSender emailSender)
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
                    Proposals = await _context.Proposal
                                        .Where(p => p.DateDeleted == null)
                                        .Where(s => s.Receiver == username)
                                        .Where(p => p.ProposalStatus != "Completed" && p.ProposalStatus != "Failed" && p.ProposalStatus != "Rejected")
                                        .Include(p => p.Project)
                                        .OrderBy(p => p.ProposalStatus)
                                        .ToListAsync();

                    ProposalCount = Proposals.Count();

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

        public async Task<IActionResult> OnPostReviewAsync(int id)
        {
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null)
            {
                ErrorMessage = "Proposal not found.";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            if (proposal.ProposalStatus != "Accepted")
            {
                ErrorMessage = "Proposal is not accepted yet";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            // update proposal status as In Review
            proposal.ProposalStatus = "In Review";
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // create new review
            var review = new Models.Review
            {
                ProjectId = proposal.ProjectId,
                ReviewStatus = "New",
                DateCreated = DateTime.Now
            };
            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            // update project status as In Review
            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);
            project.ProjectStatus = "In Review";
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(m => m.AssignedId == username);
            var coordinator = await _context.Coordinator.Where(c => c.DateDeleted == null).FirstOrDefaultAsync();

            // send email
            var mailsubject = "FYP System - Request for Review";
            var mailbody = $@"Dear Coordinator, 

I would like to request for review on my proposal:

Project ID: {project.AssignedId}
Project Name: {project.ProjectTitle}

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, coordinator.CoordinatorEmail, mailsubject, mailbody);

            SuccessMessage = "Review requested!";

            return RedirectToPage("/Supervisor/Project/ReceivedProposal");
        }

        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null)
            {
                ErrorMessage = "Proposal not found.";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            if (proposal.ProposalStatus != "Approved")
            {
                ErrorMessage = "Proposal is not approved yet";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Sender);

            // check if student had other project. if the student had other then he cannout be confirmed and the requisition status will be set as faiiled
            if (student.ProjectId != null)
            {
                // update requisition as failed
                proposal.ProposalStatus = "Failed";
                proposal.DateModified = DateTime.Now;
                await _context.SaveChangesAsync();

                // update project as n/a
                var failedProject = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);
                failedProject.ProjectStatus = "Not Available";
                failedProject.SupervisorId = null;
                failedProject.DateModified = DateTime.Now;
                await _context.SaveChangesAsync();

                ErrorMessage = "This student had confirmed with other project. You cannot accept the student.";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            // update requisition as completed
            proposal.ProposalStatus = "Completed";
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);
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

Your proposal for the following project is approved and you are confirmed with this project. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please navigate to the system menu under Project --> My Proposal or My Project to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            SuccessMessage = "Project confirmed with student.";

            return RedirectToPage("/Supervisor/Project/ReceivedProposal");
        }
    }
}