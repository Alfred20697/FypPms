using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Student.Project
{
    public class ViewProposalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ViewProposalModel> _logger;

        [BindProperty]
        public Proposal Proposal { get; set; }
        [BindProperty]
        public Models.Student Student { get; set; }
        [BindProperty]
        public Models.Project Project { get; set; }
        [BindProperty]
        public ProjectSpecialization ProjectSpecialization { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ViewProposalModel(FypPmsContext context, ILogger<ViewProposalModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    Proposal = await _context.Proposal.Where(p => p.DateDeleted == null).Include(p => p.Project).FirstOrDefaultAsync(m => m.ProposalId == id);

                    if (Proposal == null)
                    {
                        return NotFound();
                    }

                    Student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == Proposal.Sender);

                    Project = await _context.Project.Where(p => p.DateDeleted == null).FirstOrDefaultAsync(p => p.ProjectId == Proposal.ProjectId);

                    ProjectSpecialization = await _context.ProjectSpecialization.Include(ps => ps.Specialization).FirstOrDefaultAsync(ps => ps.ProjectId == Proposal.ProjectId);

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