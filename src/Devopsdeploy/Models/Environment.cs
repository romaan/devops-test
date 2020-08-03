using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevOpsDeploy.Models {
    [Table("environment")]
    public class Environment {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var environment = obj as Environment;
            if (string.IsNullOrEmpty(environment?.Id)) return false;
            return EqualityComparer<string>.Default.Equals(Id, environment.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Environment{" +
                    $"ID='{Id}'" +
                    $", Name='{Name}'" +
                    "}";
        }
    }
}
