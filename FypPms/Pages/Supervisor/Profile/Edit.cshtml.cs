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

namespace FypPms.Pages.Supervisor.Profile
{
    public class EditModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<EditModel> _logger;

        [BindProperty]
        public Models.Supervisor Supervisor { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public EditModel(FypPmsContext context, ILogger<EditModel> logger)
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
                    if (id == null)
                    {
                        return NotFound();
                    }

                    Supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).Include(s => s.User).FirstOrDefaultAsync(m => m.SupervisorId == id);

                    if (Supervisor == null)
                    {
                        return NotFound();
                    }

                    return Page();
                }
                else
                {
                    ErrorMessage = "Access Denied";
                    return RedirectToPage($"/{usertype}/Edit");
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
            
            Supervisor.DateModified = DateTime.Now;
            _context.Attach(Supervisor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupervisorExists(Supervisor.SupervisorId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            SuccessMessage = "Profile modified successfully.";

            return RedirectToPage("/Supervisor/Profile/Index");
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("/Supervisor/Profile/Index");
        }

        private bool SupervisorExists(int id)
        {
            return _context.Supervisor.Any(e => e.SupervisorId == id);
        }
    }
}