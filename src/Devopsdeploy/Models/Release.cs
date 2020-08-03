using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevOpsDeploy.Models {
    [Table("release")]
    public class Release {
        [Key]
        public string Id { get; set; }

        public string Version { get; set; }
        [Required]
        public DateTime Created { get; set; }

        [Required]
        [ForeignKey("Project")]
        public string ProjectId { get; set; }

        public List <Deployment> Deployments  { get; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var release = obj as Release;
            if (string.IsNullOrEmpty(release?.Id)) return false;
            return EqualityComparer<string>.Default.Equals(Id, release.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Release{" +
                    $"ID='{Id}'" +
                    $", Version='{Version}'" +
                    $", Created='{Created}'" +
                    "}";
        }
    }
}
