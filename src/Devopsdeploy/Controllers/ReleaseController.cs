using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOpsDeploy.Controllers.Dto;
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
    public class ReleaseController : ControllerBase {
        private const string EntityName = "release";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<ReleaseController> _log;

        public ReleaseController(ILogger<ReleaseController> log,
            ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        [HttpPost("releases")]
        [ValidateModel]
        public async Task<ActionResult<Release>> CreateRelease([FromBody] Release release)
        {
            _log.LogDebug($"REST request to save Release : {release}");
            if (string.IsNullOrEmpty(release.Id))
                throw new BadRequestAlertException("A new release cannot already have an ID", EntityName, "idexists");
            _applicationDatabaseContext.Releases.Add(release);
            await _applicationDatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRelease), new { id = release.Id }, release)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, release.Id.ToString()));
        }

        [HttpPut("releases")]
        [ValidateModel]
        public async Task<IActionResult> UpdateRelease([FromBody] Release release)
        {
            _log.LogDebug($"REST request to update Release : {release}");
            if (string.IsNullOrEmpty(release.Id)) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Releases.Update(release);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(release)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, release.Id.ToString()));
        }

        [HttpGet("releases")]
        public ActionResult<IEnumerable<Release>> GetAllReleases(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Releases");
            var page = _applicationDatabaseContext.Releases
                .UsePageable(pageable);
            return Ok(page.Content).WithHeaders(page.GeneratePaginationHttpHeaders());
        }

        [HttpGet("releases/{id}")]
        public async Task<IActionResult> GetRelease([FromRoute] string id)
        {
            _log.LogDebug($"REST request to get Release : {id}");
            var result = await _applicationDatabaseContext.Releases
                .Include(r => r.Deployments)
                .SingleOrDefaultAsync(release => release.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("releases/{id}")]
        public async Task<IActionResult> DeleteRelease([FromRoute] string id)
        {
            _log.LogDebug($"REST request to delete Release : {id}");
            _applicationDatabaseContext.Releases.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
