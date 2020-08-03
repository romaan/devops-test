using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Pagination;
using JHipsterNet.Pagination.Extensions;
using DevOpsDeploy.Data;
using DevOpsDeploy.Data.Extensions;
using DevOpsDeploy.Models;
using DevOpsDeploy.Web.Extensions;
using DevOpsDeploy.Web.Filters;
using DevOpsDeploy.Web.Rest.Problems;
using DevOpsDeploy.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevOpsDeploy.Controllers {
    [Authorize]
    [Route("api")]
    [ApiController]
    public class ProjectController : ControllerBase {
        private const string EntityName = "project";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<ProjectController> _log;

        public ProjectController(ILogger<ProjectController> log,
            ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        [HttpPost("projects")]
        [ValidateModel]
        public async Task<ActionResult<Project>> CreateProject([FromBody] Project project)
        {
            _log.LogDebug($"REST request to save Project : {project}");
            if (string.IsNullOrEmpty(project?.Id))
                throw new BadRequestAlertException("A new project cannot already have an ID", EntityName, "idexists");
            _applicationDatabaseContext.Projects.Add(project);
            await _applicationDatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, project.Id.ToString()));
        }

        [HttpPut("projects")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProject([FromBody] Project project)
        {
            _log.LogDebug($"REST request to update Project : {project}");
            if (string.IsNullOrEmpty(project?.Id)) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Update(project);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(project)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, project.Id.ToString()));
        }

        [HttpGet("projects")]
        public ActionResult<IEnumerable<Project>> GetAllProjects(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Projects");
            var page = _applicationDatabaseContext.Projects
                .UsePageable(pageable);
            return Ok(page.Content).WithHeaders(page.GeneratePaginationHttpHeaders());
        }

        [HttpGet("projects/{id}")]
        public async Task<IActionResult> GetProject([FromRoute] string id)
        {
            _log.LogDebug($"REST request to get Project : {id}");
            var result = await _applicationDatabaseContext.Projects
                .SingleOrDefaultAsync(project => project.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("projects/{id}")]
        public async Task<IActionResult> DeleteProject([FromRoute] string id)
        {
            _log.LogDebug($"REST request to delete Project : {id}");
            _applicationDatabaseContext.Projects.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
