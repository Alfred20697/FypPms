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
    public class MyProposalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private ILogger<MyProposalModel> _logger;
        private readonly IEmailSender _emailSender;

        public IList<Proposal> Proposals { get; set; }
        public int ProposalCount { get; set; }
        public IList<Proposal> CompletedProposals { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public MyProposalModel(FypPmsContext context, ILogger<MyProposalModel> logger, IEmailSender emailSender)
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
                                        .Where(p => p.ProposalStatus != "Approved" && p.ProposalStatus != "Completed")
                                        .Where(s => s.Sender == username)
                                        .Include(p => p.Project)
                                        .OrderBy(p => p.ProposalStatus == "Rejected")
                                        .ThenBy(p => p.ProposalStatus == "In Review")
                                        .ToListAsync();

                    ProposalCount = Proposals.Count();

                    CompletedProposals = await _context.Proposal
                                                .Where(p => p.DateDeleted == null)
                                                .Where(p => p.ProposalStatus == "Approved")
                                                .Where(s => s.Sender == username)
                                                .Include(p => p.Project)
                                                .OrderBy(p => p.ProposalStatus)
                                                .ToListAsync();

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

            // update proposal status
            Proposal proposal = await _context.Proposal.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(m => m.ProposalId == id);

            proposal.ProposalStatus = "In Review";
            proposal.DateModified = DateTime.Now;
            _context.Attach(proposal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProposalExists(proposal.ProposalId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Create new review
            var review = new Models.Review
            {
                ProjectId = proposal.ProjectId,
                ReviewStatus = "New",
                DateCreated = DateTime.Now
            };
            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            // Update current project
            var project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);
            project.ProjectStatus = "In Review";
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username").ToString();
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(m => m.AssignedId == username);
            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync();

            //Email Handler
            var mailsubject = "FYP System - Request for Review";
            var mailbody = $@"Dear Coordinator, 

I would like to request for review on my proposal:

Project ID: {proposal.Project.AssignedId}
Project Name: {proposal.Project.ProjectTitle}

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, coordinator.CoordinatorEmail, mailsubject, mailbody);

            // Log Message
            SuccessMessage = $"Review for {proposal.Project.ProjectTitle} requested!";
            
            return RedirectToPage("/Supervisor/Project/MyProposal");
        }

        private bool ProposalExists(int id)
        {
            return _context.Proposal.Any(e => e.ProposalId == id);
        }
    }
}