using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FypPms.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly FypPmsContext _context;
        private ILogger<LoginModel> _logger;

        public new User User { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public LoginModel(FypPmsContext context, ILogger<LoginModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class LoginData
        {
            [Required]
            [DisplayName("User Name")]
            public string UserName { get; set; }

            [Required, DataType(DataType.Password)]
            //[StringLength(16, MinimumLength = 8)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        [BindProperty]
        public LoginData loginData { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            User = await _context.User.Where(s => s.UserName == loginData.UserName).Where(s => s.UserStatus == "Active")
                            .FirstOrDefaultAsync();

            if (User != null)
            {
                if (Hash.Validate(loginData.Password, Salt.Generate(loginData.UserName), User.UserPassword)){
                    SuccessMessage = $"User {User.UserName} logged in successfully!";

                    HttpContext.Session.Clear();
                    HttpContext.Session.SetString("_username", User.UserName);
                    HttpContext.Session.SetString("_usertype", User.UserType);

                    switch (User.UserType)
                    {
                        case "Student":
                            return RedirectToPage("/Student/Index");
                        case "Supervisor":
                            return RedirectToPage("/Supervisor/Index");
                        case "Coordinator":
                            return RedirectToPage("/Coordinator/Index");
                        default:
                            return RedirectToPage("/Account/Login");
                    }
                }
                else
                {
                    ErrorMessage = $"{User.UserName} fail to log in.";
                }
            }
            else
            {
                ErrorMessage = $"{loginData.UserName} does not exist.";
            }
            
            return RedirectToPage("/Account/Login");
        }
    }
}