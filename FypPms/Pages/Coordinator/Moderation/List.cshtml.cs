using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Coordinator.Moderation
{
    public class ListModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ListModel> _logger;

        public IList<Models.Project> Projects { get; set; }
        public Dictionary<string, string> SupervisorPairs { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ListModel(FypPmsContext context, ILogger<ListModel> logger)
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
                    Projects = await _context.Project
                        .Where(p => p.DateDeleted == null)
                        .Where(p => p.ModeratorId != null)
                        .ToListAsync();
                    
                    SupervisorPairs = new Dictionary<string, string>();

                    var supervisors = await _context.Supervisor
                        .Where(s => s.DateDeleted == null)
                        .ToListAsync();

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