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

namespace FypPms.Pages.Coordinator.Announcement
{
    public class EditModel : PageModel
    {
        private readonly FypPmsContext _context;

        [BindProperty]
        public Models.Announcement Announcement { get; set; }
        public SelectList TypeSelectList { get; set; }
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

                    Announcement = await _context.Announcement.Where(a => a.DateDeleted == null).FirstOrDefaultAsync(m => m.AnnouncementId == id);

                    if (Announcement == null)
                    {
                        return NotFound();
                    }

                    TypeSelectList = new SelectList(new List<SelectListItem>
                                        {
                                            new SelectListItem { Text = "All", Value = "All"},
                                            new SelectListItem { Text = "Student", Value = "Student"},
                                            new SelectListItem { Text = "Supervisor", Value = "Supervisor"},
                                        }, "Value", "Text");

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

            Announcement.AnnouncementStatus = "Edited";
            Announcement.DateModified = DateTime.Now;
            _context.Attach(Announcement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnouncementExists(Announcement.AnnouncementId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            SuccessMessage = "Announcement modified successfully.";

            return RedirectToPage("/Coordinator/Announcement/Index");
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcement.Any(e => e.AnnouncementId == id);
        }
    }
}
