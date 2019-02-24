using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    public class ChangePasswordModel : PageModel
    {
        private readonly FypPmsContext _context;
        private ILogger<ChangePasswordModel> _logger;

        public new FypPms.Models.User User { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ChangePasswordModel(FypPmsContext context, ILogger<ChangePasswordModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class ChangeData
        {
            [Required, DataType(DataType.Password)]
            [DisplayName("Current Password")]
            public string Password { get; set; }

            [Required, DataType(DataType.Password)]
            [DisplayName("Confirm New Password")]
            public string ConfirmPassword { get; set; }

            [Required, DataType(DataType.Password)]
            [DisplayName("New Password")]
            public string NewPassword { get; set; }
        }

        [BindProperty]
        public ChangeData changeData { get; set; }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
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

            var username = HttpContext.Session.GetString("_username");

            User = await _context.User.Where(s => s.UserName == username)
                            .FirstOrDefaultAsync();
            
            if (User != null)
            {

                if (Hash.Validate(changeData.Password, Salt.Generate(username), User.UserPassword))
                {
                    if (changeData.NewPassword.Equals(changeData.ConfirmPassword))
                    {
                        var newHashedPassword = Hash.Generate(changeData.NewPassword, Salt.Generate(username));

                        User.UserPassword = newHashedPassword;
                        User.DateModified = DateTime.Now;

                        SuccessMessage = $"Password changed for " + username;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ErrorMessage = $"The two passwords do not match. ";
                    }
                }
                else
                {
                    ErrorMessage = $"The existing password is incorrect. ";
                }
            }
            else
            {
                ErrorMessage = $"User {username} does not exist.";
            }
            
            return RedirectToPage("/Coordinator/Management/Index");
        }
    }
}