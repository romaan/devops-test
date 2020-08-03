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
    public class ConfigController : ControllerBase {
        private const string EntityName = "config";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<ConfigController> _log;

        public ConfigController(ILogger<ConfigController> log,
            ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        [HttpPost("configs")]
        [ValidateModel]
        public async Task<ActionResult<ConfigController>> CreateConfig([FromBody] Config config)
        {
            _log.LogDebug($"REST request to save Config : {config}");
            if (string.IsNullOrEmpty(config.Id))
                throw new BadRequestAlertException("A new config cannot already have an ID", EntityName, "idexists");
            _applicationDatabaseContext.Configs.Add(config);
            await _applicationDatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetConfig), new { id = config.Id }, config)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, config.Id));
        }

        [HttpPut("configs")]
        [ValidateModel]
        public async Task<IActionResult> UpdateConfig([FromBody] Config config)
        {
            _log.LogDebug($"REST request to update Config : {config}");
            if (string.IsNullOrEmpty(config.Id)) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Update(config);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(config)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, config.Id.ToString()));
        }

        [HttpGet("configs")]
        public ActionResult<IEnumerable<Config>> GetAllConfigs(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Configs");
            var page = _applicationDatabaseContext.Configs
                .UsePageable(pageable);
            return Ok(page.Content).WithHeaders(page.GeneratePaginationHttpHeaders());
        }

        [HttpGet("configs/{id}")]
        public async Task<IActionResult> GetConfig([FromRoute] string id)
        {
            _log.LogDebug($"REST request to get Config : {id}");
            var result = await _applicationDatabaseContext.Configs
                .SingleOrDefaultAsync(config => config.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("configs/{id}")]
        public async Task<IActionResult> DeleteConfig([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete Config : {id}");
            _applicationDatabaseContext.Configs.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
