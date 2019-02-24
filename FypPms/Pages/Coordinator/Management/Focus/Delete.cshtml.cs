using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FypPms.Models;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages.Coordinator.Management.Focus
{
    public class DeleteModel : PageModel
    {
        private readonly FypPmsContext _context;

        [BindProperty]
        public Models.Focus Focus { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public DeleteModel(FypPmsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    Focus = await _context.Focus
                        .Where(f => f.DateDeleted == null)
                        .Include(f => f.Specialization).FirstOrDefaultAsync(m => m.FocusId == id);

                    if (Focus == null)
                    {
                        return NotFound();
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Focus = await _context.Focus.FindAsync(id);

            if (Focus != null)
            {
                Focus.DateDeleted = DateTime.Now;
                //_context.Focus.Remove(Focus);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Coordinator/Management/Focus/Index");
        }
    }
}
