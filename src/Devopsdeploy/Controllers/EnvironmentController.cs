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
    public class EnvironmentController : ControllerBase {
        private const string EntityName = "environment";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<EnvironmentController> _log;

        public EnvironmentController(ILogger<EnvironmentController> log,
            ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        [HttpPost("environments")]
        [ValidateModel]
        public async Task<ActionResult<Environment>> CreateEnvironment([FromBody] Environment environment)
        {
            _log.LogDebug($"REST request to save Environment : {environment}");
            if (string.IsNullOrEmpty(environment.Id))
                throw new BadRequestAlertException("A new environment cannot already have an ID", EntityName, "idexists");
            _applicationDatabaseContext.Environments.Add(environment);
            await _applicationDatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEnvironment), new { id = environment.Id }, environment)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, environment.Id));
        }

        [HttpPut("environments")]
        [ValidateModel]
        public async Task<IActionResult> UpdateEnvironment([FromBody] Environment environment)
        {
            _log.LogDebug($"REST request to update Environment : {environment}");
            if (string.IsNullOrEmpty(environment.Id)) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Update(environment);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(environment)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, environment.Id.ToString()));
        }

        [HttpGet("environments")]
        public ActionResult<IEnumerable<Environment>> GetAllEnvironments(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Environments");
            var page = _applicationDatabaseContext.Environments
                .UsePageable(pageable);
            return Ok(page.Content).WithHeaders(page.GeneratePaginationHttpHeaders());
        }

        [HttpGet("environments/{id}")]
        public async Task<IActionResult> GetEnvironment([FromRoute] string id)
        {
            _log.LogDebug($"REST request to get Environment : {id}");
            var result = await _applicationDatabaseContext.Environments
                .SingleOrDefaultAsync(environment => environment.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("environments/{id}")]
        public async Task<IActionResult> DeleteEnvironment([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete Environment : {id}");
            _applicationDatabaseContext.Environments.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
