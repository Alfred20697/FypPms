using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Student
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        public Models.Project Project { get; set; }
        public Models.Supervisor Supervisor { get; set; }
        public WeeklyLog WeeklyLog { get; set; }
        public SubmissionType SubmissionType { get; set; }
        public Models.Submission Submission { get; set; }
        public IList<Announcement> Announcements { get; set; }
        public Proposal Proposal { get; set; }

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
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Announcements = await _context.Announcement
                                    .Where(a => a.DateDeleted == null)
                                    .Where(a => a.AnnouncementType == "All" || a.AnnouncementType == "Student")
                                    .OrderByDescending(a => a.DateCreated)
                                    .Take(3)
                                    .ToListAsync();

                    var student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

                    if (student.ProjectId != null)
                    {
                        Project = await _context.Project
                           .Where(p => p.DateDeleted == null)
                           .FirstOrDefaultAsync(p => p.ProjectId == student.ProjectId);

                        Supervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);
                    }

                    Proposal = await _context.Proposal
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ProposalStatus == "Require Modification")
                        .Include(p => p.Project)
                        .FirstOrDefaultAsync(p => p.Sender == username);

                    WeeklyLog = await _context.WeeklyLog
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.WeeklyLogStatus == "Require Modification")
                        .Include(p => p.Project)
                        .FirstOrDefaultAsync(p => p.StudentId == username);

                    SubmissionType = await _context.SubmissionType
                        .Where(s => s.DateDeleted == null)
                        .Where(s => s.BatchId == student.BatchId)
                        .Where(s => s.StartDate <= DateTime.Now)
                        .Where(s => s.GraceDate >= DateTime.Now)
                        .FirstOrDefaultAsync();

                    if (SubmissionType != null)
                    {
                        Submission = await _context.Submission
                        .Where(s => s.DateDeleted == null)
                        .Where(s => s.SubmissionTypeId == SubmissionType.SubmissionTypeId)
                        .Where(s => s.ProjectId == student.ProjectId)
                        .FirstOrDefaultAsync();
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
            var access = new Access(username, "Student");

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