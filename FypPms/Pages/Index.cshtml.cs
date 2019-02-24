using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FypPms.Models;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace FypPms.Pages
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;

        public IList<Announcement> Announcements { get; set; }
        public new User User { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context)
        {
            _context = context;
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

        public async Task OnGetAsync()
        {
            Announcements = await _context.Announcement
                .Where(a => a.DateDeleted == null)
                .Where(a => a.AnnouncementType == "All")
                .OrderByDescending(a => a.DateCreated)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetDownloadAsync(int id)
        {
            var announcement = await _context.Announcement
                        .FirstOrDefaultAsync(a => a.AnnouncementId == id);

            var filePath = announcement.AttachmentFolder + announcement.AttachmentFile;

            var path = Path.Combine(Directory.GetCurrentDirectory(), announcement.AttachmentFolder.Substring(1, announcement.AttachmentFolder.Length - 2), announcement.AttachmentFile);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, FileService.GetContentType(path), Path.GetFileName(path));
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
                if (Hash.Validate(loginData.Password, Salt.Generate(loginData.UserName), User.UserPassword))
                {
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
