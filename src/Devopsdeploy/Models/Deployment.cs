using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevOpsDeploy.Models {
    [Table("deployment")]
    public class Deployment {
        [Key]
        public string Id { get; set; }

        [Required]
        public DateTime DeployedAt { get; set; }

        [Required]
        [ForeignKey("Release")]
        public string ReleaseId { get; set; }

        public Release Release { get; set; }

        [Required]
        [ForeignKey("Environment")]
        public string EnvironmentId { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var deployment = obj as Deployment;
            if (string.IsNullOrEmpty(deployment?.Id)) return false;
            return EqualityComparer<string>.Default.Equals(Id, deployment.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Deployment{" +
                    $"ID='{Id}'" +
                    $", DeployedAt='{DeployedAt}'" +
                    "}";
        }
    }
}
