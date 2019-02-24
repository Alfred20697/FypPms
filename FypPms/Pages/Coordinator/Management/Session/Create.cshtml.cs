﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FypPms.Models;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages.Coordinator.Management.Session
{
    public class CreateModel : PageModel
    {
        private readonly FypPmsContext _context;

        [BindProperty]
        public Models.Session Session { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public CreateModel(FypPmsContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
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

            Session.DateCreated = DateTime.Now;
            _context.Session.Add(Session);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Coordinator/Management/Session/Index");
        }
    }
}