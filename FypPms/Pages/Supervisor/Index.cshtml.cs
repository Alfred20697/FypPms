using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Supervisor
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<Models.Project> Projects { get; set; }
        public IList<WeeklyLog> WeeklyLogs { get; set; }
        public IList<Announcement> Announcements { get; set; }
        public IList<Proposal> Proposals { get; set; }
        public IList<Requisition> Requisitions { get; set; }
        public IList<Models.Review> Reviews { get; set; }
        public IList<Models.Student> Students { get; set; }
        public IList<SubmissionType> SubmissionTypes { get; set; }
        public Dictionary<string, string> StudentPairs = new Dictionary<string, string>();
        public IList<Models.Supervisor> Supervisors { get; set; }
        public Dictionary<string, string> SupervisorPairs = new Dictionary<string, string>();
        public Dictionary<int, int> SubmissionCountPairs = new Dictionary<int, int>();

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
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
                    Announcements = await _context.Announcement
                        .Where(a => a.DateDeleted == null)
                        .Where(a => a.AnnouncementType == "All" || a.AnnouncementType == "Supervisor")
                        .OrderByDescending(a => a.DateCreated)
                        .Take(3)
                        .ToListAsync();

                    Projects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.SupervisorId == username)
                        .Where(p => p.ProjectStatus == "Taken")
                        .OrderBy(p => p.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    Students = await _context.Student.Where(s => s.DateDeleted == null).ToListAsync();
                    
                    foreach (var student in Students)
                    {
                        StudentPairs.Add(student.AssignedId, student.StudentName);
                    }

                    Supervisors = await _context.Supervisor.Where(s => s.DateDeleted == null).ToListAsync();

                    foreach (var supervisor in Supervisors)
                    {
                        SupervisorPairs.Add(supervisor.AssignedId, supervisor.SupervisorName);
                    }

                    Proposals = await _context.Proposal
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ProposalStatus == "New" || p.ProposalStatus == "Modified" || p.ProposalStatus == "Require Modification")
                        .Where(p => p.Receiver == username)
                        .Include(p => p.Project)
                        .OrderBy(p => p.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    Requisitions = await _context.Requisition
                        .Where(r => r.DateDeleted == null)
                        .Where(r => r.RequisitionStatus == "New")
                        .Where(r => r.Receiver == username)
                        .Include(p => p.Project)
                        .OrderBy(r => r.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    Reviews = await _context.Review
                        .Where(r => r.DateDeleted == null)
                        .Where(r => r.ReviewStatus != "Completed")
                        .Where(r => r.Reviewer == username)
                        .Include(p => p.Project)
                        .OrderBy(r => r.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    WeeklyLogs = await _context.WeeklyLog
                        .Where(w => w.DateDeleted == null)
                        .Where(w => w.WeeklyLogStatus == "New" || w.WeeklyLogStatus == "Modified")
                        .Where(w => w.SupervisorId == username)
                        .Include(w => w.Project)
                        .OrderBy(w => w.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    SubmissionTypes = await _context.SubmissionType
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .OrderBy(s => s.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    foreach(var item in SubmissionTypes)
                    {
                        var submission = await _context.Submission
                            .Where(s => s.DateDeleted == null)
                            .Where(s => s.SubmissionTypeId == item.SubmissionTypeId)
                            .Include(s => s.Project)
                            .Where(s => s.Project.SupervisorId == username || s.Project.CoSupervisorId == username)
                            .ToListAsync();
                        
                        SubmissionCountPairs.Add(item.SubmissionTypeId, submission.Count());
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

        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    var announcement = await _context.Announcement
                        .FirstOrDefaultAsync(a => a.AnnouncementId == id);

                    var filePath = announcement.AttachmentFolder + announcement.AttachmentFile;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), announcement.AttachmentFolder.Substring(1, announcement.AttachmentFolder.Length - 2), announcement.AttachmentFile);

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    return File(memory, FileService.GetContentType(path), Path.GetFileName(path));
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
    }
}