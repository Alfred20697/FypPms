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

namespace FypPms.Pages.Coordinator.Management.SubmissionType
{
    public class EditModel : PageModel
    {
        private readonly FypPmsContext _context;

        [BindProperty]
        public Models.SubmissionType SubmissionType { get; set; }
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

                    SubmissionType = await _context.SubmissionType.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(m => m.SubmissionTypeId == id);

                    if (SubmissionType == null)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            SubmissionType.EndDate = SubmissionType.EndDate.AddDays(1).AddSeconds(-1);
            SubmissionType.GraceDate = SubmissionType.GraceDate.AddDays(1).AddSeconds(-1);
            SubmissionType.Status = "Modified";
            SubmissionType.DateModified = DateTime.Now;
            _context.Attach(SubmissionType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubmissionTypeExists(SubmissionType.SubmissionTypeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            SuccessMessage = "Submission modified successfully.";

            return RedirectToPage("/Coordinator/Management/SubmissionType/Index");
        }

        private bool SubmissionTypeExists(int id)
        {
            return _context.SubmissionType.Any(e => e.SubmissionTypeId == id);
        }
    }
}
