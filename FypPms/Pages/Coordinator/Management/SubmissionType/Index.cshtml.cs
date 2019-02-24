using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FypPms.Models;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages.Coordinator.Management.SubmissionType
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;

        public IList<Models.SubmissionType> SubmissionType { get; set; }
        public Dictionary<int?, int> SubmissionCountPairs = new Dictionary<int?, int>();

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context)
        {
            _context = context;
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
                    SubmissionType = await _context.SubmissionType
                        .Where(s => s.DateDeleted == null)
                        .Include(s => s.Batch)
                        .ToListAsync();

                    foreach(var item in SubmissionType)
                    {
                        var submission = await _context.Submission
                            .Where(s => s.DateDeleted == null)
                            .Where(s => s.SubmissionName == item.Name)
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
    }
}
