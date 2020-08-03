using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevOpsDeploy.Models {
    [Table("project")]
    public class Project {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Release> Releases { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var project = obj as Project;
            if (string.IsNullOrEmpty(project?.Id)) return false;
            return EqualityComparer<string>.Default.Equals(Id, project.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Project{" +
                    $"ID='{Id}'" +
                    $", Name='{Name}'" +
                    "}";
        }
    }
}
