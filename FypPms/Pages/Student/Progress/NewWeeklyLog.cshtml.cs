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

namespace FypPms.Pages.Student.Progress
{
    public class NewWeeklyLogModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<NewWeeklyLogModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public Models.Student Student { get; set; }
        [BindProperty]
        public Models.Supervisor Supervisor { get; set; }
        [BindProperty]
        public Models.Supervisor CoSupervisor { get; set; }
        public IList<WeeklyLog> WeeklyLogs { get; set; }
        public SelectList SessionList { get; set; }
        [BindProperty]
        public CreateForm Cf { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public NewWeeklyLogModel(FypPmsContext context, ILogger<NewWeeklyLogModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public class CreateForm
        {
            [DisplayName("Meeting Number")]
            public int WeeklyLogNumber { get; set; }
            [DisplayName("Meeting Date")]
            public DateTime Date { get; set; }
            [DisplayName("Project Session")]
            public string Session { get; set; }
            [DisplayName("Work Done")]
            public string WorkDone { get; set; }
            [DisplayName("Work To Be Done")]
            public string WorkToBeDone { get; set; }
            [DisplayName("Problems Faced")]
            public string Problem { get; set; }
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
                        .Include(s => s.Batch)
                        .Include(s => s.Project)
                        .FirstOrDefaultAsync(s => s.AssignedId == username);

                    Project = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .FirstOrDefaultAsync(p => p.ProjectId == Student.ProjectId);

                    Supervisor = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

                    if (Project.CoSupervisorId != null)
                    {
                        CoSupervisor = await _context.Supervisor
                            .Where(s => s.DateDeleted == null)
                            .FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);
                    }

                    WeeklyLogs = await _context.WeeklyLog
                        .Where(w => w.DateDeleted == null)
                        .Where(w => w.StudentId == username)
                        .ToListAsync();

                    Cf = new CreateForm
                    {
                        Date = DateTime.Now
                    };

                    var temp = WeeklyLogs.Count() + 1;

                    ViewData["Number"] = Enumerable.ToList(new[] { temp })
                                                .Select(n => new SelectListItem
                                                {
                                                    Value = n.ToString(),
                                                    Text = n.ToString()
                                                }).ToList();

                    SessionList = new SelectList(_context.Session.Where(s => s.DateDeleted == null), "SessionName", "SessionName");

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
            if (Cf.WorkDone == null)
            {
                ErrorMessage = "Work done field cannot be empty.";
                return RedirectToPage("/Student/Progress/NewWeeklyLog");
            }

            var username = HttpContext.Session.GetString("_username");

            Student = await _context.Student
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .Include(s => s.Project)
                        .FirstOrDefaultAsync(s => s.AssignedId == username);

            Project = await _context.Project
                .Where(p => p.DateDeleted == null)
                .FirstOrDefaultAsync(p => p.ProjectId == Student.ProjectId);

            Supervisor = await _context.Supervisor
                .Where(s => s.DateDeleted == null)
                .FirstOrDefaultAsync(s => s.AssignedId == Project.SupervisorId);

            if (Project.CoSupervisorId != null)
            {
                CoSupervisor = await _context.Supervisor
                    .Where(s => s.DateDeleted == null)
                    .FirstOrDefaultAsync(s => s.AssignedId == Project.CoSupervisorId);
            }
            else
            {
                CoSupervisor = null;
            }

            var stage = Cf.Session.Split(" ")[0];

            // add new weekly log
            var weeklyLog = new WeeklyLog
            {
                StudentId = Student.AssignedId,
                SupervisorId = Supervisor.AssignedId,
                CoSupervisorId = CoSupervisor?.AssignedId,
                WeeklyLogStatus = "New",
                WeeklyLogDate = Cf.Date,
                WeeklyLogNumber = Cf.WeeklyLogNumber,
                WeeklyLogSession = Cf.Session,
                WeeklyLogStage = stage,
                WorkDone = Cf.WorkDone,
                WorkToBeDone = Cf.WorkToBeDone,
                Problem = Cf.Problem,
                ProjectId = Project.ProjectId,
                DateCreated = DateTime.Now
            };
            _context.WeeklyLog.Add(weeklyLog);
            await _context.SaveChangesAsync();
            
            // send email notification
            var mailsubject = $"FYP System - Weekly Log {Cf.WeeklyLogNumber}";
            var mailbody = $@"Dear {Supervisor.SupervisorName}, 

A new weekly log had been added for the project below: 

Project ID: {Project.AssignedId}
Project Title: {Project.ProjectTitle}

Work Done:
{Cf.WorkDone}

Work to Be Done:
{Cf.WorkToBeDone}

Problems Faced:
{Cf.Problem}

Please navigate to the system menu under My Supervision --> Supervision List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{Student.StudentName}
{Student.AssignedId}
(FYP Student)";

            await _emailSender.SendEmailAsync(Student.StudentEmail,Supervisor.SupervisorEmail, mailsubject, mailbody);


            // send notificaiton to cosupervisor if exists
            if(CoSupervisor != null)
            {
                var mailsubject2 = $"FYP System - Weekly Log {Cf.WeeklyLogNumber}";
                var mailbody2 = $@"Dear {CoSupervisor.SupervisorName}, 

A new weekly log had been added for the project below: 

Project ID: {Project.AssignedId}
Project Title: {Project.ProjectTitle}

Work Done:
{Cf.WorkDone}

Work to Be Done:
{Cf.WorkToBeDone}

Problems Faced:
{Cf.Problem}

Please navigate to the system menu under My Supervision --> Supervision List to check for more information.

Please contact the Coordinator if you found any problem or difficulty using the system. Thank You.

Yours Sincerely,
{Student.StudentName}
{Student.AssignedId}
(FYP Student)";

                await _emailSender.SendEmailAsync(Student.StudentEmail, CoSupervisor.SupervisorEmail, mailsubject2, mailbody2);
            }

            // success message
            SuccessMessage = $"Weekly log {Cf.WeeklyLogNumber} created successfully";

            return RedirectToPage("/Student/Progress/Index");
        }
    }
}