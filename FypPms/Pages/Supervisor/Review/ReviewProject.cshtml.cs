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

namespace FypPms.Pages.Supervisor.Review
{
    public class ReviewProjectModel : PageModel
    {
        private readonly FypPmsContext _context;
        private ILogger<ReviewProjectModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Models.Review Review { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public List<ProjectSpecialization> ProjectSpecializations { get; set; }
        public IList<Models.Supervisor> Supervisors { get; set; }
        public Dictionary<string, string> SupervisorPairs = new Dictionary<string, string>();
        [BindProperty]
        public ReviewProjectForm Rpf { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ReviewProjectModel(FypPmsContext context, ILogger<ReviewProjectModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class ReviewProjectForm
        {
            public string Comment { get; set; }
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

                    Review = await _context.Review
                        .Include(r => r.Project).FirstOrDefaultAsync(m => m.ReviewId == id);

                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Review.ProjectId);

                    ProjectSpecializations = await _context.ProjectSpecialization.Where(p => p.ProjectId == Project.ProjectId).ToListAsync();

                    if (Review == null)
                    {
                        return NotFound();
                    }

                    Rpf = new ReviewProjectForm
                    {
                        Comment = Review.ReviewComment
                    };

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

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var review = await _context.Review.Where(r => r.DateDeleted == null).FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                ErrorMessage = "Review not found";
                return RedirectToPage("/Supervisor/Review/Index");
            }

            if (review.ReviewStatus == "Completed" || review.ReviewStatus == "Require Modification")
            {
                ErrorMessage = "Review is completed. Action denied";
                return RedirectToPage("/Supervisor/Review/Index");
            }

            // Update review status as completed, no changes to comment
            review.ReviewStatus = "Completed";
            review.ReviewComment = Rpf.Comment;
            review.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // update proposal status as approved
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == review.ProjectId);
            proposal.ProposalStatus = "Approved";
            proposal.ProposalComment = Rpf.Comment;
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // update project status as ready
            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == review.ProjectId);
            project.ProjectStatus = "Available";
            project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);
            var username = HttpContext.Session.GetString("_username");
            var reviewer = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

            // Email notification
            var mailsubject = "FYP System - Review Result (Approved)";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

Your following project had been reviewed and allowed available for students.

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Comment:
{Rpf.Comment}

Please navigate to the system menu under Project --> My Project to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{reviewer.SupervisorName}
(Assigned Reviewer)";

            await _emailSender.SendEmailAsync(reviewer.SupervisorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Sender);

            if (student != null)
            {
                var mailsubject2 = "FYP System - Review Result (Approved)";
                var mailbody2 = $@"Dear {student.StudentName}, 

Your following project had been reviewed and approved. 

Supervisor: {supervisor.SupervisorName}
Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Comment:
{Rpf.Comment}

Your supervisor will carry out next process. 

Please navigate to the system menu under Project --> My Proposal to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{reviewer.SupervisorName}
(Assigned Reviewer)";

                await _emailSender.SendEmailAsync(reviewer.SupervisorEmail, student.StudentEmail, mailsubject2, mailbody2);

                SuccessMessage = "Review approved.";

                // automate accept student

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

                // Email notification
                var mailsubject3 = "FYP System - Project Accepted";
                var mailbody3 = $@"Dear {student.StudentName}, 

Your proposal for the following project is approved and you are confirmed with this project. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please navigate to the system menu under Project --> My Proposal or My Project to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

                await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject3, mailbody3);

                var mailsubject4 = "FYP System - Project Accepted";
                var mailbody4 = $@"Dear {supervisor.SupervisorName}, 

The student proposal for the following project is approved. The student had been confirmed to you. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please navigate to the system menu under Project --> My Proposal or My Project to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{reviewer.SupervisorName}
(Assigned Reviewer)";

                await _emailSender.SendEmailAsync(reviewer.SupervisorEmail, supervisor.SupervisorEmail, mailsubject4, mailbody4);

                SuccessMessage += " Project confirmed with student.";
            }

            return RedirectToPage("/Supervisor/Review/Index");
        }

        public async Task<IActionResult> OnPostModifyAsync(int id)
        {
            if (Rpf.Comment == null)
            {
                ErrorMessage = "Comments required!";
                return RedirectToPage("/Supervisor/Review/ReviewProject", id);
            }

            // update proposal status as accepted
            var review = await _context.Review.Where(r => r.DateDeleted == null).FirstOrDefaultAsync(p => p.ReviewId == id);

            if (review == null)
            {
                ErrorMessage = "Review not found";
                return RedirectToPage("/Supervisor/Review/Index");
            }

            if (review.ReviewStatus == "Completed" || review.ReviewStatus == "Require Modification")
            {
                ErrorMessage = "Review is completed. Action denied";
                return RedirectToPage("/Supervisor/Review/Index");
            }

            // Update review status as require modification, add comment
            review.ReviewStatus = "Require Modification";
            review.ReviewComment = Rpf.Comment;
            review.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // update proposal status as Require Modification
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == review.ProjectId);
            proposal.ProposalStatus = "Require Modification";
            proposal.ProposalComment = Rpf.Comment;
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == review.ProjectId);
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);
            var username = HttpContext.Session.GetString("_username");
            var reviewer = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

            // Email notification
            var mailsubject = "FYP System - Review Result (Require Modification)";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

Your following project had been reviewed and requires modification.

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Comment:
{Rpf.Comment}

Please make changes accordingly and submit for another review.

Please navigate to the system menu under Project --> My Proposal to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{reviewer.SupervisorName}
(Assigned Reviewer)";

            await _emailSender.SendEmailAsync(reviewer.SupervisorEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Sender);

            if (student != null)
            {
                var mailsubject2 = "FYP System - Review Result (Require Modification)";
                var mailbody2 = $@"Dear {student.StudentName}, 

Your following project had been reviewed and requires modification.

Supervisor: {supervisor.SupervisorName}
Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Comment:
{Rpf.Comment}

Your supervisor will be responsible for the changes. 

Please navigate to the system menu under Project --> My Proposal to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{reviewer.SupervisorName}
(Assigned Reviewer)";

                await _emailSender.SendEmailAsync(reviewer.SupervisorEmail, student.StudentEmail, mailsubject2, mailbody2);
            }

            return RedirectToPage("/Supervisor/Review/Index");
        }
        
        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.ReviewId == id);
        }
    }
}