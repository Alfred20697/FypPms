using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public FypPms.Models.Student Student { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }

        public RegisterModel(FypPmsContext context, ILogger<RegisterModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult OnGet()
        {
            ViewData["BatchId"] = new SelectList(_context.Batch, "BatchId", "BatchName");
            ViewData["SpecializationId"] = new SelectList(_context.Specialization, "SpecializationName", "SpecializationName");
            ViewData["Numbers"] = Enumerable.ToList(new[] { "Male", "Female"})
                                    .Select(n => new SelectListItem
                                    {
                                        Value = n.ToString(),
                                        Text = n.ToString()
                                    }).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create new student
            var temp = Student.StudentName;
            Student.StudentName = temp.ToUpper();
            Student.StudentStatus = "New";
            Student.DateCreated = DateTime.Now;

            _context.Student.Add(Student);
            await _context.SaveChangesAsync();

            var coordinator = await _context.Coordinator.Where(c => c.DateDeleted == null).FirstOrDefaultAsync();

            var mailsubject = "FYP System - Student Approval";
            
            var mailbody = $@"Dear {coordinator.CoordinatorName}, 

There is a new student approval request. 

Student ID: {Student.AssignedId}
Student Name: {Student.StudentName}

Please navigate to Student --> Student Approval for more information. ";

            await _emailSender.SendEmailAsync(Student.StudentEmail, coordinator.CoordinatorEmail, mailsubject, mailbody);

            // Log Message
            SuccessMessage = $"Student {Student.StudentName} registered successfully!";

            return RedirectToPage("/Account/Login");
        }
    }
}