using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FypPms.Models;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages.Coordinator.Management.Batch
{
    public class DeleteModel : PageModel
    {
        private readonly FypPmsContext _context;
        [BindProperty]
        public Models.Batch Batch { get; set; }
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

                    Batch = await _context.Batch.Where(b => b.DateDeleted == null).FirstOrDefaultAsync(m => m.BatchId == id);

                    if (Batch == null)
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

            Batch = await _context.Batch.FindAsync(id);

            if (Batch != null)
            {
                Batch.DateDeleted = DateTime.Now;
                //_context.Batch.Remove(Batch);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Coordinator/Management/Batch/Index");
        }
    }
}
