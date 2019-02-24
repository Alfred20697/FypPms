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

namespace FypPms.Pages.Student.Project
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
        public IList<Proposal> CheckProposals { get; set; }
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

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    CheckProposals = await _context.Proposal
                                            .Where(s => s.DateDeleted == null)
                                            .Where(p => p.Sender == username)
                                            .Where(p => p.ProposalStatus != "Rejected")
                                            .ToListAsync();

                    if (CheckProposals.Count() > 0)
                    {
                        ErrorMessage = "Access denied for creating new proposal";
                        return RedirectToPage($"/Student/Project/MyProposal");
                    }

                    ViewData["Specialization"] = new SelectList(_context.Specialization, "SpecializationId", "SpecializationName");
                    ViewData["Supervisor"] = new SelectList(_context.Supervisor, "AssignedId", "SupervisorName");
                    ViewData["ProjectType"] = Enumerable.ToList(new[] { "Application-based Project", "Research-based Project", "Application & Research-based Project" })
                                                .Select(n => new SelectListItem
                                                {
                                                    Value = n.ToString(),
                                                    Text = n.ToString()
                                                }).ToList();
                    ViewData["ProjectCategory"] = new SelectList(_context.Category.OrderBy(c => c.CategoryName), "CategoryName", "CategoryName");
                    ViewData["ProjectFocus"] = new SelectList(_context.Focus.OrderBy(f => f.FocusName), "FocusName", "FocusName");

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

            //Create new project
            Project.ProjectStatus = "New";
            Project.ProposedBy = "Student";
            Project.AssignedId = projects.Count() + 1;
            Project.NumberOfStudent = 1;
            Project.ProjectCollaboration = false;
            Project.DateCreated = DateTime.Now;
            _context.Project.Add(Project);
            await _context.SaveChangesAsync();

            // Create new proposal
            Proposal.Sender = username;
            Proposal.Receiver = Project.SupervisorId;
            Proposal.ProjectId = Project.ProjectId;
            Proposal.ProposalStatus = "New";
            Proposal.DateCreated = DateTime.Now;
            _context.Proposal.Add(Proposal);
            await _context.SaveChangesAsync();

            // Create new project specialization
            ProjectSpecialization.ProjectId = Project.ProjectId;
            _context.ProjectSpecialization.Add(ProjectSpecialization);
            await _context.SaveChangesAsync();

            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);
            var specialization = await _context.Specialization.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.SpecializationId == ProjectSpecialization.SpecializationId);
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

            // Email notification to supervisor
            var mailsubject = "FYP System - Project Proposal";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

I would like to propose a project as detailed below: 

Project ID: {Project.AssignedId}
Project Title: {Project.ProjectTitle}
Project Type: {Project.ProjectType}
Project Specialization: {specialization.SpecializationName}
Project Category: {Project.ProjectCategory}
Project Focus: {Project.ProjectFocus}

Please navigate to the system menu under Project --> Received Proposal

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{student.StudentName}
{username}
(FYP Student)";

            await _emailSender.SendEmailAsync(student.StudentEmail, supervisor.SupervisorEmail, mailsubject, mailbody);
            
            SuccessMessage = "Proposal sent successfully.";

            return RedirectToPage("/Student/Project/MyProposal");
        }
    }
}