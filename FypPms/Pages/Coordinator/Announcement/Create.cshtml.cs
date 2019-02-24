using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FypPms.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace FypPms.Pages.Coordinator.Announcement
{
    public class CreateModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        [BindProperty]
        public Models.Announcement Announcement { get; set; }
        [BindProperty]
        public AttachmentForm Af { get; set; }
        public SelectList TypeSelectList { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public CreateModel(FypPmsContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public class AttachmentForm
        {
            [DisplayName("Attachment File")]
            public IFormFile AttachmentFile { get; set; }
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
                    TypeSelectList = new SelectList(new List<SelectListItem>
                                        {
                                            new SelectListItem { Text = "All", Value = "All"},
                                            new SelectListItem { Text = "Student", Value = "Student"},
                                            new SelectListItem { Text = "Supervisor", Value = "Supervisor"},
                                        }, "Value", "Text");

                    Af = new AttachmentForm();

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

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if (file != null)
            {
                var rootPath = _appEnvironment.ContentRootPath;
                var folderPath = $"\\Files\\Announcement\\";
                var filePathWithoutName = rootPath + folderPath;
                Directory.CreateDirectory(filePathWithoutName);
                var fileName = file.FileName;
                var filePath = filePathWithoutName + fileName;
                using (var fstream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fstream);
                }

                Announcement.AttachmentFolder = folderPath;
                Announcement.AttachmentFile = fileName;
            }

            Announcement.AnnouncementStatus = "New";
            Announcement.DateCreated = DateTime.Now;
            _context.Announcement.Add(Announcement);
            await _context.SaveChangesAsync();

            SuccessMessage = $"File: {file.FileName} Announcement created successfully.";

            return RedirectToPage("/Coordinator/Announcement/Index");
        }
    }
}