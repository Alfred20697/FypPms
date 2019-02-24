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

namespace FypPms.Pages.Coordinator.Profile
{
    public class EditProfileModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<EditProfileModel> _logger;

        [BindProperty]
        public Models.Coordinator Coordinator { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public EditProfileModel(FypPmsContext context, ILogger<EditProfileModel> logger)
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
                    Coordinator = await _context.Coordinator
                        .Where(c => c.DateDeleted == null)
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.AssignedId == username);
                    
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

            Coordinator.DateModified = DateTime.Now;
            _context.Attach(Coordinator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoordinatorExists(Coordinator.CoordinatorId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            SuccessMessage = "Profile modified successfully.";

            return RedirectToPage("/Coordinator/Management/Index");
        }
        
        private bool CoordinatorExists(int id)
        {
            return _context.Coordinator.Any(e => e.CoordinatorId == id);
        }
    }
}