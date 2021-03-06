﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FypPms.Models;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages.Coordinator.Management.Specialization
{
    public class EditModel : PageModel
    {
        private readonly FypPmsContext _context;

        [BindProperty]
        public Models.Specialization Specialization { get; set; }
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

                    Specialization = await _context.Specialization.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(m => m.SpecializationId == id);

                    if (Specialization == null)
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

            Specialization.DateModified = DateTime.Now;
            _context.Attach(Specialization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecializationExists(Specialization.SpecializationId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Coordinator/Management/Specialization/Index");
        }

        private bool SpecializationExists(int id)
        {
            return _context.Specialization.Any(e => e.SpecializationId == id);
        }
    }
}
