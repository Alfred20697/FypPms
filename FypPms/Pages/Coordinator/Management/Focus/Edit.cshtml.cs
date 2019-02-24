using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FypPms.Models;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages.Coordinator.Management.Focus
{
    public class EditModel : PageModel
    {
        private readonly FypPmsContext _context;

        [BindProperty]
        public Models.Focus Focus { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public EditModel(FypPmsContext context)
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

                    ViewData["SpecializationId"] = new SelectList(_context.Specialization, "SpecializationId", "SpecializationName");

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

            Focus.DateModified = DateTime.Now;
            _context.Attach(Focus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FocusExists(Focus.FocusId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Coordinator/Management/Focus/Index");
        }

        private bool FocusExists(int id)
        {
            return _context.Focus.Any(e => e.FocusId == id);
        }
    }
}
