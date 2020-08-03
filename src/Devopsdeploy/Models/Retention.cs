using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevOpsDeploy.Models {
    [Table("retention")]
    public class Retention {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Release")]
        public string ReleaseId { get; set; }

        public Release Release { get; set; }

        [Required]
        [ForeignKey("Environment")]
        public string EnvironmentId { get; set; }

        public Environment Environment { get; set; }

        [Required]
        [ForeignKey("Project")]
        public string ProjectId { get; set; }

        public Project Project { get; set; }

        [Required]
        [ForeignKey("Deployment")]
        public string DeploymentId { get; set; }

        public Deployment Deployment { get; set; }


        [Required] public DateTime Created { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var retention = obj as Retention;
            if (retention?.Id == null) return false;
            return EqualityComparer<long>.Default.Equals(Id, retention.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Config{" +
                   $"ID='{Id}'" +
                   $",Created='{Created}'" +
                   "}";
        }
    }
}
