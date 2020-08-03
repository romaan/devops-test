using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Pagination;
using JHipsterNet.Pagination.Extensions;
using DevOpsDeploy.Data;
using DevOpsDeploy.Data.Extensions;
using DevOpsDeploy.Models;
using DevOpsDeploy.Service;
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
    public class DeploymentController : ControllerBase {
        private const string EntityName = "deployment";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<DeploymentController> _log;

        private readonly IReleaseCleanupService _releaseCleanupService;

        public DeploymentController(ILogger<DeploymentController> log,
            ApplicationDatabaseContext applicationDatabaseContext, IReleaseCleanupService releaseCleanupService)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
            _releaseCleanupService = releaseCleanupService;
        }

        [HttpPost("deployments")]
        [ValidateModel]
        public async Task<ActionResult<Deployment>> CreateDeployment([FromBody] Deployment deployment)
        {
            _log.LogDebug($"REST request to save Deployment : {deployment}");
            if (string.IsNullOrEmpty(deployment.Id))
                throw new BadRequestAlertException("A new deployment cannot already have an ID", EntityName, "idexists");
             _applicationDatabaseContext.Deployments.Add(deployment);
            await _applicationDatabaseContext.SaveChangesAsync();
            _releaseCleanupService.PerformCleanup(deployment);
            return CreatedAtAction(nameof(GetDeployment), new { id = deployment.Id }, deployment)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, deployment.Id));
        }

        [HttpPut("deployments")]
        [ValidateModel]
        public async Task<IActionResult> UpdateDeployment([FromBody] Deployment deployment)
        {
            _log.LogDebug($"REST request to update Deployment : {deployment}");
            if (string.IsNullOrEmpty(deployment.Id)) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Update(deployment);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(deployment)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, deployment.Id.ToString()));
        }

        [HttpGet("deployments")]
        public ActionResult<IEnumerable<Deployment>> GetAllDeployments(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Deployments");
            var page = _applicationDatabaseContext.Deployments
                .UsePageable(pageable);
            return Ok(page.Content).WithHeaders(page.GeneratePaginationHttpHeaders());
        }

        [HttpGet("deployments/{id}")]
        public async Task<IActionResult> GetDeployment([FromRoute] string id)
        {
            _log.LogDebug($"REST request to get Deployment : {id}");
            var result = await _applicationDatabaseContext.Deployments
                .SingleOrDefaultAsync(deployment => deployment.Id.Equals(id));
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("deployments/{id}")]
        public async Task<IActionResult> DeleteDeployment([FromRoute] string id)
        {
            _log.LogDebug($"REST request to delete Deployment : {id}");
            _applicationDatabaseContext.Deployments.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }


    }
}
