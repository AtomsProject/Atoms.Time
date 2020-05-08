﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atom.TimeTracker.Database;
using Atom.TimeTracker.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Atom.TimeTracker.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly TimeSheetContext _context;

        public ProjectsController(TimeSheetContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IEnumerable<Project>> GetProjects(string searchTerm = null, bool showAll = false)
        {
            IQueryable<Project> q = _context.Projects;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                q = q.Where(p => p.Name.Contains(searchTerm));
            }
            else if (!showAll)
            {
                q = q.Where(p => p.IsArchived == false && p.TimeSheetEntries.Any(s => s.TimeSheet.Person.UserName == this.UserName()));
            }

            return await q.ToListAsync();
        }

        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();
            return project;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> PostCreateProject([FromBody]ProjectContent content)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Name == content.Name);
            if (project != null)
                return Conflict("Project with this name already exists.");

            project = new Project
            {
                Name = content.Name,
                IsRnD = content.IsRnD ?? false,
                IsArchived = content.IsObsolete ?? false
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PostUpdateProject(int id, [FromBody]ProjectContent content)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
                return NotFound();

            var hasChange = false;
            content.Name = content.Name?.Trim();

            if (!string.IsNullOrWhiteSpace(content.Name)
                && !content.Name.Equals(project.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await _context.Projects.AnyAsync(p => p.Name == content.Name))
                    return Conflict("Project with this name already exists.");

                project.Name = content.Name;
                hasChange = true;
            }

            if (content.IsRnD.HasValue)
            {
                project.IsRnD = content.IsRnD.Value;
                hasChange = true;
            }

            if (content.IsObsolete.HasValue)
            {
                project.IsArchived = content.IsObsolete.Value;
                hasChange = true;
            }

            if (hasChange)
            {
                await _context.SaveChangesAsync();
            }

            return Ok(project);
        }

        public class ProjectContent
        {
            public string Name { get; set; }
            public bool? IsRnD { get; set; }
            public bool? IsObsolete { get; set; }
        }
    }
}