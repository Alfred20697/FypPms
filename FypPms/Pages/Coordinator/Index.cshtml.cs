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

namespace FypPms.Pages.Coordinator
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<Models.Announcement> Announcements { get; set; }
        public IList<Models.Review> Reviews { get; set; }
        public IList<Models.Student> Students { get; set; }
        public IList<Models.Supervisor> Supervisors { get; set; }
        public IList<Models.ChangeRequest> ChangeRequests { get; set; }
        public Dictionary<string, string> SupervisorPairs = new Dictionary<string, string>();

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
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Announcements = await _context.Announcement
                        .Where(a => a.DateDeleted == null)
                        .Where(a => a.AnnouncementType == "All" || a.AnnouncementType == "Supervisor" || a.AnnouncementType == "Student")
                        .OrderByDescending(a => a.DateCreated)
                        .Take(3)
                        .ToListAsync();

                    Reviews = await _context.Review
                        .Where(r => r.DateDeleted == null)
                        .Where(r => r.ReviewStatus == "New")
                        .Include(r => r.Project)
                        .OrderByDescending(r => r.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    Students = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Where(s => s.StudentStatus == "New")
                        .OrderByDescending(s => s.DateCreated)
                        .Take(5)
                        .ToListAsync();

                    ChangeRequests = await _context.ChangeRequest
                        .Where(c => c.DateDeleted == null)
                        .Where(c => c.ChangeRequestStatus == "New")
                        .Include(c => c.Project)
                        .OrderByDescending(c => c.DateCreated)
                        .Take(5)
                        .ToListAsync();

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

        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

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