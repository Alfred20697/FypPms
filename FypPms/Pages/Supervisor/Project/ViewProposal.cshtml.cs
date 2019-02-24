using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class ViewProposalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewProposalModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Proposal Proposal { get; set; }
        [BindProperty]
        public Models.Student Student { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public ProjectSpecialization ProjectSpecialization { get; set; }
        public SelectList CoSupervisorSelectList { get; set; }
        [BindProperty]
        public ViewProposalForm Vpf { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public class ViewProposalForm
        {
            public string Comment { get; set; }
            [DisplayName("Moderator 1")]
            public string ModeratorOne { get; set; }
            [DisplayName("Moderator 2")]
            public string ModeratorTwo { get; set; }
            [DisplayName("Moderator 3")]
            public string ModeratorThree { get; set; }
            [DisplayName("Co-Supervisor")]
            public string CoSupervisor { get; set; }
        }

        public ViewProposalModel(FypPmsContext context, ILogger<ViewProposalModel> logger, IEmailSender emailSender)
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

                    Proposal = await _context.Proposal.Where(p => p.DateDeleted == null).Include(p => p.Project).FirstOrDefaultAsync(m => m.ProposalId == id);

                    if (Proposal == null)
                    {
                        return NotFound();
                    }

                    Student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Proposal.Sender);

                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Proposal.ProjectId);

                    ProjectSpecialization = await _context.ProjectSpecialization.Include(ps => ps.Specialization).FirstOrDefaultAsync(ps => ps.ProjectId == Proposal.ProjectId);

                    Vpf = new ViewProposalForm
                    {
                        Comment = Proposal.ProposalComment,
                        ModeratorOne = Project.ModeratorOne,
                        ModeratorTwo = Project.ModeratorTwo,
                        ModeratorThree = Project.ModeratorThree,
                        CoSupervisor = Project.CoSupervisorId
                    };

                    CoSupervisorSelectList = new SelectList(_context.Supervisor.Where(s => s.DateDeleted == null).Where(s => s.AssignedId != username), "AssignedId", "SupervisorName");

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
            // update proposal status as accepted
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null)
            {
                ErrorMessage = "Proposal not found";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            if (proposal.ProposalStatus != "New" && proposal.ProposalStatus != "Modified")
            {
                ErrorMessage = "Proposal had been accepted or rejected. Action denied.";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            proposal.ProposalStatus = "Accepted";
            proposal.ProposalComment = Vpf.Comment;
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Sender);
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);
            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo == null && Vpf.ModeratorThree == null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne == null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree == null)
            {
                project.ModeratorOne = Vpf.ModeratorTwo;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne == null && Vpf.ModeratorTwo == null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree == null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                project.ModeratorTwo = Vpf.ModeratorTwo;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne == null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorTwo;
                project.ModeratorTwo = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo == null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                project.ModeratorTwo = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                project.ModeratorTwo = Vpf.ModeratorTwo;
                project.ModeratorThree = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.CoSupervisor != null)
            {
                project.CoSupervisorId = Vpf.CoSupervisor;
                await _context.SaveChangesAsync();

                var cosupervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Vpf.CoSupervisor);

                var mailsubject2 = "FYP System - Assigned as Co-Supervisor";
                var mailbody2 = $@"Dear {cosupervisor.SupervisorName}, 

You had been assigned as Co-Supervisor for the following project. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Please navigate to the system menu under Project --> My Project to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

                await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, cosupervisor.SupervisorEmail, mailsubject2, mailbody2);
            }

            // send email
            var mailsubject = "FYP System - Proposal Accepted";
            var mailbody = $@"Dear {student.StudentName}, 

Your proposal for the following project is accepted. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Comments: 
{Vpf.Comment}

The project will be sent for review before you are confirmed with the project. 

Please navigate to the system menu under Project --> My Proposal to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            SuccessMessage = "Proposal approved.";

            // Automatic request for review
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
            project.ProjectStatus = "In Review";
            await _context.SaveChangesAsync();
            
            var coordinator = await _context.Coordinator.Where(c => c.DateDeleted == null).FirstOrDefaultAsync();

            // send email
            var mailsubject3 = "FYP System - Request for Review";
            var mailbody3 = $@"Dear Coordinator, 

I would like to request for review on my proposal:

Project ID: {project.AssignedId}
Project Name: {project.ProjectTitle}

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, coordinator.CoordinatorEmail, mailsubject3, mailbody3);

            SuccessMessage += " Review requested!";

            return RedirectToPage("/Supervisor/Project/ReceivedProposal");
        }

        public async Task<IActionResult> OnPostModifyAsync(int id)
        {
            if (Vpf.Comment == null)
            {
                ErrorMessage = "Comments required!";
                return RedirectToPage("/Supervisor/Project/ViewProposal", id);
            }

            // update proposal status as accepted
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null)
            {
                ErrorMessage = "Proposal not found.";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            if (proposal.ProposalStatus != "New" && proposal.ProposalStatus != "Modified")
            {
                ErrorMessage = "Proposal had been accepted or rejected. Action denied.";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            proposal.ProposalStatus = "Require Modification";
            proposal.ProposalComment = Vpf.Comment;
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Sender);
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);
            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo == null && Vpf.ModeratorThree == null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne == null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree == null)
            {
                project.ModeratorOne = Vpf.ModeratorTwo;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne == null && Vpf.ModeratorTwo == null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree == null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                project.ModeratorTwo = Vpf.ModeratorTwo;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne == null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorTwo;
                project.ModeratorTwo = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo == null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                project.ModeratorTwo = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.ModeratorOne != null && Vpf.ModeratorTwo != null && Vpf.ModeratorThree != null)
            {
                project.ModeratorOne = Vpf.ModeratorOne;
                project.ModeratorTwo = Vpf.ModeratorTwo;
                project.ModeratorThree = Vpf.ModeratorThree;
                await _context.SaveChangesAsync();
            }

            if (Vpf.CoSupervisor != null)
            {
                project.CoSupervisorId = Vpf.CoSupervisor;
                await _context.SaveChangesAsync();
            }

            // send email
            var mailsubject = "FYP System - Proposal Require Modification";
            var mailbody = $@"Dear {student.StudentName}, 

Your proposal for the following project needs further modification. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Comments:
{Vpf.Comment}

You are required to modify your proposal and resubmit for further action. Please make changes as mentioned above. Thank you.  

Please navigate to the system menu under Project --> My Proposal to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            ErrorMessage = "Proposal require modification.";
            return RedirectToPage("/Supervisor/Project/ReceivedProposal");
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            if (Vpf.Comment == null)
            {
                ErrorMessage = "Comments required!";
                return RedirectToPage("/Supervisor/Project/ViewProposal", id);
            }

            // update proposal status as rejected
            var proposal = await _context.Proposal.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProposalId == id);

            if (proposal == null)
            {
                ErrorMessage = "Proposal not found";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            if (proposal.ProposalStatus != "New" && proposal.ProposalStatus != "Modified")
            {
                ErrorMessage = "Proposal had been accepted or rejected. Action denied.";
                return RedirectToPage("/Supervisor/Project/ReceivedProposal");
            }

            proposal.ProposalStatus = "Rejected";
            proposal.ProposalComment = Vpf.Comment;
            proposal.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            var username = HttpContext.Session.GetString("_username");
            var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == proposal.Sender);
            var supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);
            var project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == proposal.ProjectId);

            project.ProjectStatus = "Not Available";
            project.DateModified = DateTime.Now;
            await _context.SaveChangesAsync();

            // send email
            var mailsubject = "FYP System - Proposal Rejected";
            var mailbody = $@"Dear {student.StudentName}, 

Your proposal for the following project is rejected. 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}
Comments: 
{Vpf.Comment}

Please navigate to the system menu under Project --> My Proposal to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{supervisor.SupervisorName}
(FYP Supervisor)";

            await _emailSender.SendEmailAsync(supervisor.SupervisorEmail, student.StudentEmail, mailsubject, mailbody);

            ErrorMessage = "Proposal rejected.";

            return RedirectToPage("/Supervisor/Project/ReceivedProposal");
        }
    }
}