using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevOpsDeploy.Models {
    [Table("config")]
    public class Config {
        [Key]
        public string Id { get; set; }

        [Required]
        public int Value { get; set; }

        // jhipster-needle-entity-add-field - JHipster will add fields here, do not remove

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var config = obj as Config;
            if (string.IsNullOrEmpty(config?.Id)) return false;
            return EqualityComparer<string>.Default.Equals(Id, config.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Config{" +
                    $"ID='{Id}'" +
                    $",Value='{Value}'" +
                    "}";
        }
    }
}
