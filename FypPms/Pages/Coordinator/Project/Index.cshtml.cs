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

namespace FypPms.Pages.Coordinator.Project
{
    public class IndexModel : PageModel
    {
        private readonly FypPmsContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<Models.Project> Projects { get; set; }
        public IList<ProjectSpecialization> ProjectSpecializations { get; set; }
        public Dictionary<string, string> SupervisorPairs { get; set; }
        public SelectList SpecializationSelectList { get; set; }
        public SelectList SupervisorSelectList { get; set; }
        public SelectList TypeSelectList { get; set; }
        public SelectList StatusSelectList { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(FypPmsContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string specializationStr, string supervisorStr, string typeStr, string statusStr)
        {
            var username = HttpContext.Session.GetString("_username");
            var usertype = HttpContext.Session.GetString("_usertype");
            var access = new Access(username, "Coordinator");

            if (access.IsLogin())
            {
                if (access.IsAuthorize(usertype))
                {
                    ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

                    // 4C0 = 1
                    if (specializationStr == null && supervisorStr == null && typeStr == null && statusStr == null)
                    {
                        Projects = await GetProject();
                    }

                    // 4C1 = 4
                    if (specializationStr != null && supervisorStr == null && typeStr == null && statusStr == null)
                    {
                        Projects = await GetProjectBySpecialization(specializationStr);
                    }

                    if (specializationStr == null && supervisorStr != null && typeStr == null && statusStr == null)
                    {
                        Projects = await GetProjectBySupervisor(supervisorStr);
                    }

                    if (specializationStr == null && supervisorStr == null && typeStr != null && statusStr == null)
                    {
                        Projects = await GetProjectByType(typeStr);
                    }

                    if (specializationStr == null && supervisorStr == null && typeStr == null && statusStr != null)
                    {
                        Projects = await GetProjectByStatus(statusStr);
                    }

                    // 4C2
                    if (specializationStr != null && supervisorStr != null && typeStr == null && statusStr == null)
                    {
                        Projects = await GetProjectBySpecializationAndSupervisor(specializationStr, supervisorStr);
                    }

                    if (specializationStr == null && supervisorStr != null && typeStr != null && statusStr == null)
                    {
                        Projects = await GetProjectBySupervisorAndType(supervisorStr, typeStr);
                    }

                    if (specializationStr != null && supervisorStr == null && typeStr != null && statusStr == null)
                    {
                        Projects = await GetProjectBySpecializationAndType(specializationStr, typeStr);
                    }

                    if (specializationStr == null && supervisorStr == null && typeStr != null && statusStr != null)
                    {
                        Projects = await GetProjectByTypeAndStatus(typeStr, statusStr);
                    }

                    if (specializationStr != null && supervisorStr == null && typeStr == null && statusStr != null)
                    {
                        Projects = await GetProjectBySpecializationAndStatus(specializationStr, statusStr);
                    }

                    if (specializationStr == null && supervisorStr != null && typeStr == null && statusStr != null)
                    {
                        Projects = await GetProjectBySupervisorAndStatus(supervisorStr, typeStr);
                    }

                    // 4C3 = 4
                    if (specializationStr != null && supervisorStr != null && typeStr != null && statusStr == null)
                    {
                        Projects = await GetProjectBySpecializationAndSupervisorAndType(specializationStr, supervisorStr, typeStr);
                    }

                    if (specializationStr == null && supervisorStr != null && typeStr != null && statusStr != null)
                    {
                        Projects = await GetProjectBySupervisorAndTypeAndStatus(supervisorStr, typeStr, statusStr);
                    }

                    if (specializationStr != null && supervisorStr == null && typeStr != null && statusStr != null)
                    {
                        Projects = await GetProjectBySpecializationAndTypeAndStatus(specializationStr, typeStr, statusStr);
                    }

                    if (specializationStr != null && supervisorStr != null && typeStr == null && statusStr != null)
                    {
                        Projects = await GetProjectBySpecializationAndSupervisorAndStatus(specializationStr, supervisorStr, statusStr);
                    }

                    // 4C4 = 1
                    if (specializationStr != null && supervisorStr != null && typeStr != null && statusStr != null)
                    {
                        Projects = await GetProjectByAll(specializationStr, supervisorStr, typeStr, statusStr);
                    }

                    var supervisors = await _context.Supervisor.Where(s => s.DateDeleted == null).ToListAsync();

                    SupervisorPairs = new Dictionary<string, string>();

                    foreach (var supervisor in supervisors)
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

                    StatusSelectList = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Text = "New", Value = "New"},
                        new SelectListItem { Text = "Available", Value = "Available"},
                        new SelectListItem { Text = "Taken", Value = "Taken"},
                        new SelectListItem { Text = "Not Available", Value = "Not Available"},
                        new SelectListItem { Text = "In Review", Value = "In Review"}
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

        // 4C0 = 1
        public async Task<IList<Models.Project>> GetProject()
        {
            Projects = await _context.Project
                            .Where(s => s.DateDeleted == null)
                            .Include(p => p.ProjectSpecialization)
                            .OrderBy(p => p.ProjectStatus)
                            .ToListAsync();

            return Projects;
        }

        // 4C1 = 4
        public async Task<IList<Models.Project>> GetProjectBySpecialization(string specializationStr)
        {
            ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

            Projects = await _context.Project
                            .Where(s => s.DateDeleted == null)
                            .Include(p => p.ProjectSpecialization)
                            .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySupervisor(string supervisorStr)
        {
            Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Include(p => p.ProjectSpecialization)
                           .OrderBy(p => p.ProjectStatus)
                           .ToListAsync();

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectByType(string typeStr)
        {
            Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.ProjectType == typeStr)
                           .Include(p => p.ProjectSpecialization)
                           .OrderBy(p => p.ProjectStatus)
                           .ToListAsync();

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectByStatus(string statusStr)
        {
            Projects = await _context.Project
                            .Where(s => s.DateDeleted == null)
                            .Where(p => p.ProjectStatus == statusStr)
                            .Include(p => p.ProjectSpecialization)
                            .OrderBy(p => p.ProjectStatus)
                            .ToListAsync();

            return Projects;
        }

        // 4C2 = 6
        public async Task<IList<Models.Project>> GetProjectBySpecializationAndSupervisor(string specializationStr, string supervisorStr)
        {
            Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Include(p => p.ProjectSpecialization)
                           .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySupervisorAndType(string supervisorStr, string typeStr)
        {
            Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Where(p => p.ProjectType == typeStr)
                           .Include(p => p.ProjectSpecialization)
                           .OrderBy(p => p.ProjectStatus)
                           .ToListAsync();

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySpecializationAndType(string specializationStr, string typeStr)
        {
            ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

            Projects = await _context.Project
                          .Where(s => s.DateDeleted == null)
                          .Where(p => p.ProjectType == typeStr)
                          .Include(p => p.ProjectSpecialization)
                          .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectByTypeAndStatus(string typeStr, string statusStr)
        {
            Projects = await _context.Project
                            .Where(s => s.DateDeleted == null)
                            .Where(p => p.ProjectType == typeStr)
                            .Where(p => p.ProjectStatus == statusStr)
                            .Include(p => p.ProjectSpecialization)
                            .OrderBy(p => p.ProjectStatus)
                            .ToListAsync();

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySpecializationAndStatus(string specializationStr, string statusStr)
        {
            ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

            Projects = await _context.Project
                            .Where(s => s.DateDeleted == null)
                            .Where(p => p.ProjectStatus == statusStr)
                            .Include(p => p.ProjectSpecialization)
                            .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySupervisorAndStatus(string supervisorStr, string statusStr)
        {
            Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.ProjectStatus == statusStr)
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Include(p => p.ProjectSpecialization)
                           .OrderBy(p => p.ProjectStatus)
                           .ToListAsync();

            return Projects;
        }

        // 4C3 = 4
        public async Task<IList<Models.Project>> GetProjectBySpecializationAndSupervisorAndType(string specializationStr, string supervisorStr, string typeStr)
        {
            ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

            Projects = await _context.Project
                          .Where(s => s.DateDeleted == null)
                          .Where(p => p.SupervisorId == supervisorStr)
                          .Where(p => p.ProjectType == typeStr)
                          .Include(p => p.ProjectSpecialization)
                          .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySupervisorAndTypeAndStatus(string supervisorStr, string typeStr, string statusStr)
        {
            Projects = await _context.Project
                           .Where(s => s.DateDeleted == null)
                           .Where(p => p.SupervisorId == supervisorStr)
                           .Where(p => p.ProjectType == typeStr)
                           .Where(p => p.ProjectStatus == statusStr)
                           .Include(p => p.ProjectSpecialization)
                           .OrderBy(p => p.ProjectStatus)
                           .ToListAsync();

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySpecializationAndTypeAndStatus(string specializationStr, string typeStr, string statusStr)
        {
            Projects = await _context.Project
                          .Where(s => s.DateDeleted == null)
                          .Where(p => p.ProjectStatus == statusStr)
                          .Where(p => p.ProjectType == typeStr)
                          .Include(p => p.ProjectSpecialization)
                          .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }

        public async Task<IList<Models.Project>> GetProjectBySpecializationAndSupervisorAndStatus(string specializationStr, string supervisorStr, string statusStr)
        {
            ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

            Projects = await _context.Project
                          .Where(s => s.DateDeleted == null)
                          .Where(p => p.ProjectStatus == statusStr)
                          .Where(p => p.SupervisorId == supervisorStr)
                          .Include(p => p.ProjectSpecialization)
                          .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }

        // 4C4
        public async Task<IList<Models.Project>> GetProjectByAll(string specializationStr, string supervisorStr, string typeStr, string statusStr)
        {
            ProjectSpecializations = await _context.ProjectSpecialization
                        .Include(ps => ps.Specialization)
                        .ToListAsync();

            Projects = await _context.Project
                          .Where(s => s.DateDeleted == null)
                          .Where(p => p.ProjectType == typeStr)
                          .Where(p => p.ProjectStatus == statusStr)
                          .Where(p => p.SupervisorId == supervisorStr)
                          .Include(p => p.ProjectSpecialization)
                          .OrderBy(p => p.ProjectStatus)
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

            return Projects;
        }
    }
}