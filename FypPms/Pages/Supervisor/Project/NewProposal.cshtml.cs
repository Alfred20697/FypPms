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

namespace FypPms.Pages.Supervisor.Project
{
    public class NewProposalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<NewProposalModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public Proposal Proposal { get; set; }
        [BindProperty]
        public ProjectSpecialization ProjectSpecialization { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public NewProposalModel(FypPmsContext context, ILogger<NewProposalModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    ViewData["Specialization"] = new SelectList(_context.Specialization.Where(s => s.DateDeleted == null), "SpecializationId", "SpecializationName");
                    ViewData["CoSupervisor"] = new SelectList(_context.Supervisor.Where(s => s.DateDeleted == null), "AssignedId", "SupervisorName");
                    ViewData["ProjectType"] = Enumerable.ToList(new[] { "Application-based Project", "Research-based Project", "Application & Research-based Project" })
                                                .Select(n => new SelectListItem
                                                {
                                                    Value = n.ToString(),
                                                    Text = n.ToString()
                                                }).ToList();
                    ViewData["NumberOfStudent"] = Enumerable.ToList(new[] { 1 })
                                                .Select(n => new SelectListItem
                                                {
                                                    Value = n.ToString(),
                                                    Text = n.ToString()
                                                }).ToList();
                    ViewData["ProjectCategory"] = new SelectList(_context.Category.Where(s => s.DateDeleted == null).OrderBy(c => c.CategoryName), "CategoryName", "CategoryName");
                    ViewData["ProjectFocus"] = new SelectList(_context.Focus.Where(s => s.DateDeleted == null).OrderBy(f => f.FocusName), "FocusName", "FocusName");

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


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var username = HttpContext.Session.GetString("_username");

            var projects = await _context.Project.ToListAsync();

            // Create new project
            Project.ProjectStatus = "New";
            Project.DateCreated = DateTime.Now;
            Project.SupervisorId = username;
            Project.ProposedBy = "Lecturer";
            Project.AssignedId = projects.Count() + 1;
            _context.Project.Add(Project);
            await _context.SaveChangesAsync();

            // Create new proposal
            Proposal.Sender = username;
            Proposal.ProjectId = Project.ProjectId;
            Proposal.ProposalStatus = "New";
            Proposal.DateCreated = DateTime.Now;
            _context.Proposal.Add(Proposal);
            await _context.SaveChangesAsync();

            // Create new project specialization
            ProjectSpecialization.ProjectId = Project.ProjectId;
            _context.ProjectSpecialization.Add(ProjectSpecialization);
            await _context.SaveChangesAsync();

            // send email to co supervisor
            if (Project.CoSupervisorId != null)
            {
                var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

                var cosupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);
                
                var mailsubject = "FYP System - Assigned as Co-Supervisor";
                var mailbody = $@"Dear {cosupervisor.SupervisorName}, 

You had been assigned as Co-Supervisor for the following project. 

Project ID: {Project.AssignedId}
Project Title: {Project.ProjectTitle}

Please navigate to the system menu under Project --> My Project to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

                await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, cosupervisor.SupervisorEmail, mailsubject, mailbody);
            }

            SuccessMessage = "New proposal created.";

            // request for review automatically
            Proposal.ProposalStatus = "In Review";
            Proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // Create new review
            var review = new Models.Review
            {
                ProjectId = Proposal.ProjectId,
                ReviewStatus = "New",
                DateCreated = DateTime.Now
            };
            _context.Review.Add(review);
            await _context.SaveChangesAsync();

            // Update current project
            var project = await _context.Project.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Proposal.ProjectId);
            project.ProjectStatus = "In Review";
            await _context.SaveChangesAsync();

            var supervisor2 = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(m => m.AssignedId == username);
            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync();

            //Email Handler
            var mailsubject2 = "FYP System - Request for Review";
            var mailbody2 = $@"Dear Coordinator, 

I would like to request for review on my proposal:

Project ID: {Proposal.Project.AssignedId}
Project Name: {Proposal.Project.ProjectTitle}

Yours Sincerely,
{supervisor2.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor2.SupervisorEmail, coordinator.CoordinatorEmail, mailsubject2, mailbody2);

            // Log Message
            SuccessMessage += $" Review for {Proposal.Project.ProjectTitle} requested!";

            return RedirectToPage("/Supervisor/Project/MyProposal");
        }
    }
}