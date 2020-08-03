using System;
using System.Linq;
using System.Threading.Tasks;
using DevOpsDeploy.Data;
using DevOpsDeploy.Models;
using Microsoft.Extensions.Logging;

namespace DevOpsDeploy.Service
{

    public interface IReleaseCleanupService
    {
        void PerformCleanup(Deployment deployment);
    }

    public class ReleaseCleanupService: IReleaseCleanupService
    {

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<IReleaseCleanupService> _log;

        private readonly string RETENTION_COUNT_ID = "ReleaseRetentionCount";


        public ReleaseCleanupService(ILogger<ReleaseCleanupService> log,
            ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        public void PerformCleanup(Deployment deployment)
        {
            int retentionCountValue = _applicationDatabaseContext.Configs.Where(r => r.Id == RETENTION_COUNT_ID)
                .Select(r => r.Value).First();
            _updateRetention(deployment, retentionCountValue);
            _deleteOlderReleases();
        }

        private void _deleteOlderReleases()
        {
            var deleteRelease = _applicationDatabaseContext.Releases
                .Where(release => !_applicationDatabaseContext.Retentions
                    .Select(retention => retention.ReleaseId).Contains(release.Id));
            _applicationDatabaseContext.Releases.RemoveRange(deleteRelease.ToList());
            _applicationDatabaseContext.SaveChangesAsync();
        }

        private void _updateRetention(Deployment deployment, int retentionCount)
        {
            var environmentId = deployment.EnvironmentId;
            var projectId = _applicationDatabaseContext.Releases
                .Where(r => r.Id.Equals(deployment.ReleaseId))
                .Select(r => r.ProjectId).Single();
            var releaseId = deployment.ReleaseId;
            var existingRelease = _applicationDatabaseContext.Retentions
                                              .FirstOrDefault(r => r.EnvironmentId.Equals(environmentId)
                                                              && r.ProjectId.Equals(projectId)
                                                              && r.ReleaseId.Equals(releaseId));
            if (existingRelease != null)
            {
                existingRelease.Created = DateTime.Now;
                existingRelease.DeploymentId = deployment.Id;
            }
            else
            {
                var cleanRetentions = _applicationDatabaseContext.Retentions
                    .Where(r => r.EnvironmentId.Equals(environmentId) && r.ProjectId.Equals(projectId))
                    .OrderByDescending(r => r.Created)
                    .Skip(retentionCount - 1);
                foreach (var cr in cleanRetentions)
                {
                    _applicationDatabaseContext.Retentions.Remove(cr);
                }

                _applicationDatabaseContext.Retentions.Add(new Retention
                {
                    EnvironmentId = environmentId,
                    ProjectId = projectId,
                    ReleaseId = releaseId,
                    Created = DateTime.Now,
                    DeploymentId = deployment.Id
                });
            }
            _applicationDatabaseContext.SaveChanges();
        }

    }
}
