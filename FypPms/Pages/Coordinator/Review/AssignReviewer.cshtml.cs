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
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Review
{
    public class AssignReviewerModel : PageModel
    {
        private readonly FypPmsContext _context;
        private ILogger<AssignReviewerModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Models.Review Review { get; set; }
        [BindProperty]
        public ProjectSpecialization ProjectSpecialization { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        public IList<Models.Supervisor> Supervisors { get; set; }
        public Dictionary<string, string> SupervisorPairs = new Dictionary<string, string>();
        public Dictionary<string, int> SupervisorReviewPairs { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public AssignReviewerModel(FypPmsContext context, ILogger<AssignReviewerModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }
        public class ReviewData
        {
            [Required]
            [DisplayName("Reviewer")]
            public string Reviewer { get; set; }
        }

        [BindProperty]
        public ReviewData reviewData { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    Review = await _context.Review
                                    .Include(r => r.Project).FirstOrDefaultAsync(m => m.ReviewId == id);

                    if (Review == null)
                    {
                        return NotFound();
                    }

                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Review.ProjectId);

                    ProjectSpecialization = await _context.ProjectSpecialization.Include(ps => ps.Specialization).FirstOrDefaultAsync(ps => ps.ProjectId == Project.ProjectId);

                    ViewData["Reviewer"] = new SelectList(_context.Supervisor.Where(s => s.AssignedId != Review.Project.SupervisorId).Where(s => s.AssignedId != Review.Project.CoSupervisorId), "AssignedId", "SupervisorName");
                    
                    Supervisors = await _context.Supervisor.Where(s => s.DateDeleted == null).ToListAsync();

                    foreach (var supervisor in Supervisors)
                    {
                        SupervisorPairs.Add(supervisor.AssignedId, supervisor.SupervisorName);
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

        public async Task<JsonResult> OnGetCount()
        {
            SupervisorReviewPairs = new Dictionary<string, int>();

            var supervisors = await _context.Supervisor
                .Where(s => s.DateDeleted == null)
                .ToListAsync();

            foreach (var item in supervisors)
            {
                var reviews = await _context.Review.Where(p => p.DateDeleted == null).Where(p => p.ReviewStatus != "Completed").Where(p => p.Reviewer == item.AssignedId).ToListAsync();

                SupervisorReviewPairs.Add(item.AssignedId, reviews.Count());
            }

            return new JsonResult(SupervisorReviewPairs);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(reviewData.Reviewer == null)
            {
                ErrorMessage = $"Reviewer cannot be null!";

                return RedirectToPage("/Coordinator/Review/AssignReviewer", Review.ReviewId);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update review, set reviewer
            _context.Review.Attach(Review);
            Review.ReviewStatus = "In Review";
            Review.DateModified = DateTime.Now;
            Review.Reviewer = reviewData.Reviewer;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(Review.ReviewId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == reviewData.Reviewer);
            var username = HttpContext.Session.GetString("_username");

            // Update project status
            var project = await _context.Project.FirstOrDefaultAsync(p => p.ProjectId == Review.ProjectId);
            project.ProjectStatus = "In Review";
            await _context.SaveChangesAsync();
            
            var projectspecialization = await _context.ProjectSpecialization.FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId);
            var specialization = await _context.Specialization.FirstOrDefaultAsync(s => s.SpecializationId == projectspecialization.SpecializationId);
            var coordinator = await _context.Coordinator.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(c => c.AssignedId == username);

            var mailsubject = "FYP System - Assigned Project to Review";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

You have been assigned to review the project as detailed below: 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Project Type: {project.ProjectType}
Project Specialization: {specialization.SpecializationName}
Project Category: {project.ProjectCategory}
Project Focus: {project.ProjectFocus}

Please navigate to the system menu under Review --> Review List

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.


Yours Sincerely,
{coordinator.CoordinatorName}
(FYP Coordinator)";

            await _emailSender.SendEmailAsync(coordinator.CoordinatorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            SuccessMessage = $"Review sent to {supervisor.SupervisorName}!";

            return RedirectToPage("/Coordinator/Review/Index");
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.ReviewId == id);
        }
    }
}