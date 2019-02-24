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
    public class MyProposalModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<MyProposalModel> _logger;

        public IList<Proposal> Proposals { get; set; }
        public IList<Proposal> ActiveProposals { get; set; }
        public IList<Proposal> RejectedProposals { get; set; }
        public Models.Student Student { get; set; }
        public Models.Requisition Requisition { get; set; }
        public int ProposalCount { get; set; }
        public int ActiveProposalCount { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public MyProposalModel(FypPmsContext context, ILogger<MyProposalModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    Proposals = await _context.Proposal
                                        .Where(s => s.DateDeleted == null)
                                        .Where(s => s.Sender == username)
                                        .Include(p => p.Project)
                                        .OrderBy(p => p.ProposalStatus)
                                        .ToListAsync();

                    ProposalCount = Proposals.Count();

                    ActiveProposals = new List<Proposal>();
                    RejectedProposals = new List<Proposal>();

                    Student = await _context.Student.Where(s => s.DateDeleted == null).FirstOrDefaultAsync(s => s.AssignedId == username);

                    foreach (var proposal in Proposals)
                    {
                        if (proposal.ProposalStatus != "Rejected" && proposal.ProposalStatus != "Failed")
                        {
                            ActiveProposalCount += 1;
                            ActiveProposals.Add(proposal);
                        }
                        else
                        {
                            RejectedProposals.Add(proposal);
                        }
                    }

                    Requisition = await _context.Requisition.Where(r => r.DateDeleted == null).Where(r => r.RequisitionStatus != "Rejected").FirstOrDefaultAsync(r => r.Sender == username);

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