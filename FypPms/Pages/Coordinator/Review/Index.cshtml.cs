using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using FypPms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Review
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IEmailSender _emailSender;
        
        public Dictionary<string, string> SupervisorPairs { get; set; }
        [BindProperty]
        public IList<Models.Review> Reviews { get; set; }
        [BindProperty]
        public IList<Models.Review> CompletedReviews { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public int ReviewCount { get; set; }
        public int CompletedCount { get; set; }

        public IndexModel(FypPmsContext context, ILogger<IndexModel> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
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
                    Reviews = await _context.Review
                                .Where(s => s.DateDeleted == null)
                                .Where(s => s.ReviewStatus != "Completed")
                                .Include(s => s.Project)
                                .OrderBy(s => s.ReviewStatus)
                                .ToListAsync();

                    ReviewCount = Reviews.Count();

                    CompletedReviews = await _context.Review
                                .Where(s => s.DateDeleted == null)
                                .Where(s => s.ReviewStatus == "Completed")
                                .Include(s => s.Project)
                                .ToListAsync();

                    CompletedCount = CompletedReviews.Count();

                    var supervisors = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .ToListAsync();

                    SupervisorPairs = new Dictionary<string, string>();

                    foreach (var item in supervisors)
                    {
                        SupervisorPairs.Add(item.AssignedId, item.SupervisorName);
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