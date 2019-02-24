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

namespace FypPms.Pages.Student.Submission
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        public Models.Student Student { get; set; }
        public Models.Project Project { get; set; }
        public SubmissionType SubmissionType { get; set; }
        public IList<Models.Submission> Submissions { get; set; }
        public bool HasSubmit { get; set; }
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
                    Student = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == username);

                    SubmissionType = await _context.SubmissionType
                       .Where(s => s.DateDeleted == null)
                       .Where(s => s.BatchId == Student.BatchId)
                       .Where(s => s.StartDate <= DateTime.Now)
                       .Where(s => s.GraceDate >= DateTime.Now)
                       .FirstOrDefaultAsync();

                    // check if submitted
                    HasSubmit = false;

                    if (Student.ProjectId != null)
                    {
                        Project = await _context.Project
                            .Where(p => p.DateDeleted == null)
                            .FirstOrDefaultAsync(p => p.ProjectId == Student.ProjectId);

                        Submissions = await _context.Submission
                            .Where(s => s.DateDeleted == null)
                            .Where(w => w.ProjectId == Project.ProjectId)
                            .Include(w => w.Project)
                            .ToListAsync();

                        if (Submissions.Count() > 0 && SubmissionType != null)
                        {
                            foreach (var submission in Submissions)
                            {
                                if (submission.SubmissionTypeId == SubmissionType.SubmissionTypeId)
                                {
                                    HasSubmit = true;
                                    break;
                                }
                            }
                        }
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