using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Student.Submission
{
    public class NewSubmissionModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<NewSubmissionModel> _logger;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public SubmissionType SubmissionType { get; set; }
        [BindProperty]
        public SubmissionForm Sf { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public NewSubmissionModel(FypPmsContext context, ILogger<NewSubmissionModel> logger, IHostingEnvironment appEnvironment, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _appEnvironment = appEnvironment;
            _emailSender = emailSender;
        }

        public class SubmissionForm
        {
            public string Batch { get; set; }
            public string Type { get; set; }
            [DisplayName("Report File")]
            public IFormFile ReportFile { get; set; }
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
                    var student = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Project)
                        .FirstOrDefaultAsync(s => s.AssignedId == username);
                    
                    SubmissionType = await _context.SubmissionType
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .FirstOrDefaultAsync(s => s.SubmissionTypeId == id);

                    var stage = "";
                    
                    if (SubmissionType.Name == "Interim Report")
                    {
                        stage = "FYP1";
                    }
                    if (SubmissionType.Name == "Final Report" || SubmissionType.Name == "Final Report (Hard Cover)")
                    {
                        stage = "FYP2";
                    }

                    var weeklyLogs = await _context.WeeklyLog
                        .Where(w => w.DateDeleted == null)
                        .Where(w => w.StudentId == username)
                        .Where(w => w.WeeklyLogStage == stage)
                        .Where(w => w.WeeklyLogStatus == "Approved")
                        .ToListAsync();

                    if (weeklyLogs.Count() < 6)
                    {
                        ErrorMessage = "You need to submit at least 6 approved weekly logs. Submission denied";
                        return RedirectToPage("/Student/Submission/Index");
                    }

                    Sf = new SubmissionForm
                    {
                        Type = SubmissionType.Name,
                        Batch = SubmissionType.Batch.BatchName
                    };

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
            if (Sf.ReportFile == null || Sf.ReportFile.Length == 0)
            {
                ErrorMessage = "No file selected";
                return RedirectToPage("/Student/Submission/Index");
            }

            if (Sf.ReportFile.Length >= 52428800)
            {
                ErrorMessage = "The file should not exceed 50MB.";
                return RedirectToPage("/Student/Submission/Index");
            }

            var submissionType = await _context.SubmissionType
                .FirstOrDefaultAsync(s => s.SubmissionTypeId == SubmissionType.SubmissionTypeId);

            if (submissionType.GraceDate < DateTime.Now || submissionType.StartDate > DateTime.Now)
            {
                ErrorMessage = "Report submission denied.";
                return RedirectToPage("/Student/Submission/Index");
            }

            var username = HttpContext.Session.GetString("_username");

            var rootPath = _appEnvironment.ContentRootPath;

            var folderPath = $"\\Files\\{username}\\";

            var filePathWithoutName = rootPath + folderPath;

            Directory.CreateDirectory(filePathWithoutName);

            var fileName = Sf.ReportFile.FileName;

            var filePath = filePathWithoutName + fileName;

            using (var fstream = new FileStream(filePath, FileMode.Create))
            {
                await Sf.ReportFile.CopyToAsync(fstream);
            }

            var student = await _context.Student
                .Where(s => s.DateDeleted == null)
                .FirstOrDefaultAsync(s => s.AssignedId == username);

            var project = await _context.Project
                .Where(p => p.DateDeleted == null)
                .FirstOrDefaultAsync(p => p.ProjectId == student.ProjectId);

            var status = "";

            if (submissionType.StartDate <= DateTime.Now && submissionType.EndDate >= DateTime.Now)
            {
                status = "New";
            }

            if (submissionType.EndDate <= DateTime.Now && submissionType.GraceDate >= DateTime.Now)
            {
                status = "Late";
            }

            var submission = new Models.Submission()
            {
                SubmissionName = Sf.Type,
                SubmissionStatus = status,
                SubmissionFolder = folderPath,
                SubmissionFile = fileName,
                SubmissionSize = Sf.ReportFile.Length,
                UploadDate = DateTime.Now,
                ProjectId = project.ProjectId,
                SubmissionTypeId = SubmissionType.SubmissionTypeId,
                DateCreated = DateTime.Now
            };

            _context.Submission.Add(submission);
            await _context.SaveChangesAsync();

            var supervisor = await _context.Supervisor
                .Where(s => s.DateDeleted == null)
                .FirstOrDefaultAsync(s => s.AssignedId == project.SupervisorId);

            var mailsubject = $"FYP System - Submission for {Sf.Type}";
            var mailbody = $@"Dear {supervisor.SupervisorName}, 

The {Sf.Type} had been submitted for the project below: 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Submission Details
Batch: {Sf.Batch}
Submission Type: {submission.SubmissionName}
Submission File Name: {submission.SubmissionFile}
Submmisionn Upload Date: {submission.UploadDate}

Please navigate to the system menu under My Supervision --> Supervision List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{student.StudentName}
{student.AssignedId}
(FYP Student)";

            await _emailSender.SendEmailAsync(student.StudentEmail, supervisor.SupervisorEmail, mailsubject, mailbody);

            var cosupervisor = await _context.Supervisor
                .Where(s => s.DateDeleted == null)
                .FirstOrDefaultAsync(s => s.AssignedId == project.CoSupervisorId);

            if (cosupervisor != null)
            {
                var mailsubject2 = $"FYP System - Submission for {Sf.Type}";
                var mailbody2 = $@"Dear {cosupervisor.SupervisorName}, 

The {Sf.Type} had been submitted for the project below: 

Project ID: {project.AssignedId}
Project Title: {project.ProjectTitle}

Submission Details
Batch: {Sf.Batch}
Submission Type: {submission.SubmissionName}
Submission File Name: {submission.SubmissionFile}
Submmisionn Upload Date: {submission.UploadDate}

Please navigate to the system menu under My Supervision --> Supervision List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{student.StudentName}
{student.AssignedId}
(FYP Student)";

                await _emailSender.SendEmailAsync(student.StudentEmail, cosupervisor.SupervisorEmail, mailsubject2, mailbody2);
            }

            SuccessMessage = $"{submission.SubmissionName} for project submitted successfully.";

            return RedirectToPage("/Student/Submission/Index");
        }
    }
}