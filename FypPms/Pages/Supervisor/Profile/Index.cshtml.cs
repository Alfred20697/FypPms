﻿using System;
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
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public Models.Supervisor Supervisor { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Supervisor");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Supervisor = await _context.Supervisor.Where(s => s.DateDeleted == null).Include(s => s.User).FirstOrDefaultAsync(s => s.AssignedId == username);

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