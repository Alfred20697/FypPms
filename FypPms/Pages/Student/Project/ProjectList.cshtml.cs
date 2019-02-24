using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FypPms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FypPms.Pages.Student.Project
{
    public class ProjectListModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<ProjectListModel> _logger;

        public IList<Models.Project> Projects { get; set; }
        public IList<Models.Supervisor> Supervisors { get; set; }
        public Dictionary<string, string> SupervisorPairs = new Dictionary<string, string>();
        public IList<ProjectSpecialization> ProjectSpecializations { get; set; }
        public SelectList SpecializationSelectList { get; set; }
        public SelectList SupervisorSelectList { get; set; }
        public SelectList TypeSelectList { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ProjectListModel(FypPmsContext context, ILogger<ProjectListModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string specializationStr, string supervisorStr, string typeStr)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Student");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

                    if (specializationStr == null && supervisorStr == null && typeStr == null)
                    {
                        Projects = await _context.Project
                            .Where(s => s.DateDeleted == null)
                            .Where(p => p.ProjectStatus.Equals("Available"))
                            .Include(p => p.ProjectSpecialization)
                            .ToListAsync();
                    }

                    if (specializationStr != null && supervisorStr == null && typeStr == null)
                    {
                        Projects = await _context.Project
                            .Where(s => s.DateDeleted == null)
                            .Where(p => p.ProjectStatus.Equals("Available"))
                            .Include(p => p.ProjectSpecialization)
                            .ToListAsync();

                        var removeList = new List<Models.Project>();

                        foreach (var project in Projects)
                        {
                            if (ProjectSpecializations.First(x => x.ProjectId == project.ProjectId).Specialization.SpecializationName != specializationStr)
                            {
                                removeList.Add(project);
                            }
                        }

                        foreach (var project in removeList)
                        {
                            Projects.Remove(project);
                        }
                    }

                    if (specializationStr == null && supervisorStr != null && typeStr == null)
                    {
                        Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.ProjectStatus.Equals("Available"))
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Include(p => p.ProjectSpecialization)
                           .ToListAsync();
                    }

                    if (specializationStr == null && supervisorStr == null && typeStr != null)
                    {
                        Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.ProjectStatus.Equals("Available"))
                           .Where(p => p.ProjectType == typeStr)
                           .Include(p => p.ProjectSpecialization)
                           .ToListAsync();
                    }

                    if (specializationStr != null && supervisorStr != null && typeStr == null)
                    {
                        Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.ProjectStatus.Equals("Available"))
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Include(p => p.ProjectSpecialization)
                           .ToListAsync();

                        var removeList = new List<Models.Project>();

                        foreach (var project in Projects)
                        {
                            if (ProjectSpecializations.First(x => x.ProjectId == project.ProjectId).Specialization.SpecializationName != specializationStr)
                            {
                                removeList.Add(project);
                            }
                        }

                        foreach (var project in removeList)
                        {
                            Projects.Remove(project);
                        }
                    }

                    if (specializationStr == null && supervisorStr != null && typeStr != null)
                    {
                        Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.ProjectStatus.Equals("Available"))
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Where(p => p.ProjectType == typeStr)
                           .Include(p => p.ProjectSpecialization)
                           .ToListAsync();
                    }

                    if (specializationStr != null && supervisorStr == null && typeStr != null)
                    {
                        Projects = await _context.Project
                          .Where(s => s.DateDeleted == null)
                          .Where(p => p.ProjectStatus.Equals("Available"))
                          .Where(p => p.ProjectType == typeStr)
                          .Include(p => p.ProjectSpecialization)
                          .ToListAsync();

                        var removeList = new List<Models.Project>();

                        foreach (var project in Projects)
                        {
                            if (ProjectSpecializations.First(x => x.ProjectId == project.ProjectId).Specialization.SpecializationName != specializationStr)
                            {
                                removeList.Add(project);
                            }
                        }

                        foreach (var project in removeList)
                        {
                            Projects.Remove(project);
                        }
                    }

                    if (specializationStr != null && supervisorStr != null && typeStr != null)
                    {
                        Projects = await _context.Project
                          .Where(s => s.DateDeleted == null)
                          .Where(p => p.ProjectStatus.Equals("Available"))
                          .Where(p => p.SupervisorId == supervisorStr)
                          .Where(p => p.ProjectType == typeStr)
                          .Include(p => p.ProjectSpecialization)
                          .ToListAsync();

                        var removeList = new List<Models.Project>();

                        foreach (var project in Projects)
                        {
                            if (ProjectSpecializations.First(x => x.ProjectId == project.ProjectId).Specialization.SpecializationName != specializationStr)
                            {
                                removeList.Add(project);
                            }
                        }

                        foreach (var project in removeList)
                        {
                            Projects.Remove(project);
                        }
                    }

                    Supervisors = await _context.Supervisor.Where(s => s.DateDeleted == null).ToListAsync();

                    foreach (var supervisor in Supervisors)
                    {
                        SupervisorPairs.Add(supervisor.AssignedId, supervisor.SupervisorName);
                    }

                    SpecializationSelectList = new SelectList(_context.Specialization.Where(s => s.DateDeleted == null), "SpecializationName", "SpecializationName");

                    SupervisorSelectList = new SelectList(_context.Supervisor.Where(s => s.DateDeleted == null), "AssignedId", "SupervisorName");

                    TypeSelectList = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Application-based Project", Value = "Application-based Project"},
                        new SelectListItem { Text = "Research-based Project", Value = "Research-based Project"},
                        new SelectListItem { Text = "Application & Research-based Project", Value = "Application & Research-based Project"},
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
    }
}