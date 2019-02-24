using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FypPms.Models;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages.Coordinator.Management.SubmissionType
{
    public class CreateModel : PageModel
    {
        private readonly FypPmsContext _context;

        [BindProperty]
        public Models.SubmissionType SubmissionType { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public CreateModel(FypPmsContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    ViewData["BatchId"] = new SelectList(_context.Batch.Where(b => b.DateDeleted == null), "BatchId", "BatchName");
                    ViewData["ReportType"] = Enumerable.ToList(new[] { "Interim Report", "Final Report", "Final Report (Hard Cover)" })
                                                .Select(n => new SelectListItem
                                                {
                                                    Value = n.ToString(),
                                                    Text = n.ToString()
                                                }).ToList();

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SubmissionType.EndDate = SubmissionType.EndDate.AddDays(1).AddSeconds(-1);
            SubmissionType.GraceDate = SubmissionType.GraceDate.AddDays(1).AddSeconds(-1);
            SubmissionType.DateCreated = DateTime.Now;
            SubmissionType.Status = "New";
            _context.SubmissionType.Add(SubmissionType);
            await _context.SaveChangesAsync();

            SuccessMessage = "Submission type created successfully.";

            return RedirectToPage("/Coordinator/Management/SubmissionType/Index");
        }
    }
}