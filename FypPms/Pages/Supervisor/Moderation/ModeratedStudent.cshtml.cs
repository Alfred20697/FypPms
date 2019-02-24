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

namespace FypPms.Pages.Supervisor.Moderation
{
    public class ModeratedStudentModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ModeratedStudentModel> _logger;

        public IList<Submission> Submissions { get; set; }
        public IList<WeeklyLog> WeeklyLogs { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ModeratedStudentModel(FypPmsContext context, ILogger<ModeratedStudentModel> logger)
        {
            _context = context;
            _logger = logger;
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
                    WeeklyLogs = await _context.WeeklyLog
                       .Where(w => w.DateDeleted == null)
                       .Where(w => w.ProjectId == id)
                       .Include(w => w.Project)
                       .ToListAsync();

                    Submissions = await _context.Submission
                            .Where(s => s.DateDeleted == null)
                            .Where(w => w.ProjectId == id)
                            .Include(w => w.Project)
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

        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    var submission = await _context.Submission
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.SubmissionId == id);

                    var filePath = submission.SubmissionFolder + submission.SubmissionFile;

                    var path = Path.Combine(Directory.GetCurrentDirectory(), submission.SubmissionFolder.Substring(1, submission.SubmissionFolder.Length - 2), submission.SubmissionFile);

                    Console.WriteLine("path: " + path);

                    var memory = new MemoryStream();
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    return File(memory, GetContentType(path), Path.GetFileName(path));
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

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip", "application/zip"},
                {".rar", "application/x-rar-compressed"},
                {".7z", "application/x-7z-compressed"}
            };
        }
    }
}