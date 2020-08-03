using System;
using DevOpsDeploy.Models;
using Microsoft.EntityFrameworkCore;
using Environment = DevOpsDeploy.Models.Environment;

namespace DevOpsDeploy.Data.Extensions
{
    public static class EntityBuilderExtensions
    {
        public static void SeedEnvironments(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Environment>().HasData(
                new Environment {Id = "Environment-1", Name = "Staging"},
                new Environment {Id = "Environment-2", Name = "Production"}
            );
        }

        public static void SeedProjects(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>().HasData(
                new Project {Id = "Project-1", Name = "Random Quotes"},
                new Project {Id = "Project-2", Name = "Pet Shop"}
            );
        }

        public static void SeedReleases(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Release>().HasData(
                new Release {Id = "Release-1", ProjectId = "Project-1", Version = "1.0.0", Created = DateTime.Parse("2000-01-01T09:00:00")},
                new Release {Id = "Release-2", ProjectId = "Project-1", Version = "1.0.1", Created = DateTime.Parse("2000-01-02T09:00:00")},
                new Release {Id = "Release-3", ProjectId = "Project-1", Version = null, Created = DateTime.Parse("2000-01-02T13:00:00")},
                new Release {Id = "Release-4", ProjectId = "Project-2", Version = "1.0.0", Created = DateTime.Parse("2000-01-01T09:00:00")},
                new Release {Id = "Release-5", ProjectId = "Project-2", Version = "1.0.1-ci1", Created = DateTime.Parse("2000-01-01T10:00:00")},
                new Release {Id = "Release-6", ProjectId = "Project-2", Version = "1.0.2", Created = DateTime.Parse("2000-01-02T09:00:00")},
                new Release {Id = "Release-7", ProjectId = "Project-2", Version = "1.0.3", Created = DateTime.Parse("2000-01-02T12:00:00")},
                new Release {Id = "Release-8", ProjectId = "Project-2", Version = "2.0.0", Created = DateTime.Parse("2000-01-01T09:00:00")}
                );
        }

        public static void SeedDeployments(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Deployment>().HasData(
                new Deployment {Id = "Deployment-1", ReleaseId = "Release-1", EnvironmentId = "Environment-1", DeployedAt = DateTime.Parse("2000-01-01T10:00:00")},
                new Deployment {Id = "Deployment-2", ReleaseId = "Release-2", EnvironmentId = "Environment-1", DeployedAt = DateTime.Parse("2000-01-02T10:00:00")},
                new Deployment {Id = "Deployment-3", ReleaseId = "Release-1", EnvironmentId = "Environment-2", DeployedAt = DateTime.Parse("2000-01-02T11:00:00")},
                new Deployment {Id = "Deployment-4", ReleaseId = "Release-2", EnvironmentId = "Environment-2", DeployedAt = DateTime.Parse("2000-01-02T12:00:00")},
                new Deployment {Id = "Deployment-5", ReleaseId = "Release-5", EnvironmentId = "Environment-1", DeployedAt = DateTime.Parse("2000-01-01T11:00:00")},
                new Deployment {Id = "Deployment-6", ReleaseId = "Release-6", EnvironmentId = "Environment-1", DeployedAt = DateTime.Parse("2000-01-02T10:00:00")},
                new Deployment {Id = "Deployment-7", ReleaseId = "Release-6", EnvironmentId = "Environment-2", DeployedAt = DateTime.Parse("2000-01-02T11:00:00")},
                new Deployment {Id = "Deployment-8", ReleaseId = "Release-7", EnvironmentId = "Environment-1", DeployedAt = DateTime.Parse("2000-01-02T13:00:00")},
                new Deployment {Id = "Deployment-9", ReleaseId = "Release-6", EnvironmentId = "Environment-1", DeployedAt = DateTime.Parse("2000-01-02T14:00:00")},
                new Deployment {Id = "Deployment-10",ReleaseId = "Release-8", EnvironmentId = "Environment-1", DeployedAt = DateTime.Parse("2000-01-01T10:00:00")}
            );
        }

        public static void SeedConfigs(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Config>().HasData(
                new Config {Id = "ReleaseRetentionCount", Value = 2}
            );
        }
    }
}
