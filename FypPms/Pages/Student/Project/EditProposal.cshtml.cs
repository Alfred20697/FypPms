using System;
using System.Collections.Generic;
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

namespace FypPms.Pages.Student.Project
{
    public class EditProposalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<EditProposalModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Proposal Proposal { get; set; }
        [BindProperty]
        public Models.Student Student { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public Models.Supervisor Supervisor { get; set; }
        [BindProperty]
        public Models.Supervisor CoSupervisor { get; set; }
        [BindProperty]
        public List<ProjectSpecialization> ProjectSpecializations { get; set; }
        
        [BindProperty]
        public EditProposalForm Epf { get; set; }
        public SelectList Categories { get; set; }
        public SelectList Focuses { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public EditProposalModel(FypPmsContext context, ILogger<EditProposalModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class EditProposalForm
        {
            [Required]
            public string Description { get; set; }
            [Required]
            public string Objective { get; set; }
            [Required]
            public string Scope { get; set; }
            public string Category { get; set; }
            public string Focus { get; set; }
            [Required]
            public string Title { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Epf = new EditProposalForm();

                    Proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProposalId == id);

                    //Student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Proposal.ProjectId);

                    if (Project != null)
                    {
                        Supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                        CoSupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);

                        ProjectSpecializations = await _context.ProjectSpecialization.Include(p => p.Specialization).Where(p => p.ProjectId == Project.ProjectId).ToListAsync();

                        Epf.Description = ProjectSpecializations[0].ProjectDescription;
                        Epf.Objective = ProjectSpecializations[0].ProjectObjective;
                        Epf.Scope = ProjectSpecializations[0].ProjectScope;
                        
                        Focuses = new SelectList(_context.Focus.Where(f => f.DateDeleted == null && f.SpecializationId == ProjectSpecializations[0].SpecializationId), "FocusName", "FocusName");
                        Epf.Focus = Project.ProjectFocus;

                        Categories = new SelectList(_context.Category.Where(c => c.DateDeleted == null && c.SpecializationId == ProjectSpecializations[0].SpecializationId), "CategoryName", "CategoryName");
                        Epf.Category = Project.ProjectCategory;
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
            if (Epf.Description == null || Epf.Objective == null || Epf.Scope == null || Epf.Title == null)
            {
                ErrorMessage = "The field cannot be empty.";
                return RedirectToPage("/Student/Project/EditProposal", id);
            }

            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null)
            {
                ErrorMessage = "Proposal not found";
                return RedirectToPage("/Student/Project/MyProposal");
            }

            if (proposal.ProposalStatus != "Require Modification")
            {
                ErrorMessage = "Modification not required. Action denied";
                return RedirectToPage("/Student/Project/MyProposal");
            }

            proposal.ProposalStatus = "Modified";
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);
            project.ProjectTitle = Epf.Title;

            project.ProjectCategory = Epf.Category;
            project.ProjectFocus = Epf.Focus;
            project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var projectSpecializations = await _context.ProjectSpecialization.Include(p => p.Specialization).Where(ps => ps.ProjectId == project.ProjectId).ToListAsync();

            projectSpecializations[0].ProjectDescription = Epf.Description;
            projectSpecializations[0].ProjectObjective = Epf.Objective;
            projectSpecializations[0].ProjectScope = Epf.Scope;
            await _context.SaveChangesAsync();

            //var review = await _context.Review.Where(r => r.DateDeleted == null).FirstOrDefaultAsync(r => r.ProjectId == project.ProjectId);
            //review.ReviewStatus = "Modified";
            //review.DateModified = DateTime.Now;
            //await _context.SaveChangesAsync();
            
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Receiver);
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Sender);

            var mailsubject = "FYP System - Proposal (Modified)";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

My proposal for the following project had been modified.

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please navigate to the system menu under Project --> Received Proposal to check for more information and take the next action.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{student.StudentName}
{student.AssignedId}
(FYP Student)";

            await _emailSender.SendEmailAsync(student.StudentEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            SuccessMessage = $"Proposal modified and sent successfully.";

            return RedirectToPage("/Student/Project/MyProposal");
        }
    }
}