using System.Xml.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Newtonsoft.Json;

namespace Models
{
    [Serializable]
    public class Mapping
    {
        //{"layer_id":6,"layer_name":"ADB","db_column_name":"network_id",
        //"db_column_data_type":"varchar","is_mandatory":true,"max_length":100,"is_dropdown":false}

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonProperty("id")]
        [Column("id")]
        public string Id { get; set; }
        [Column("layer_id")]
        [JsonProperty("layer_id")]
        public int LayerId { get; set; }
        [Column("db_column_name")]
        [JsonProperty("db_column_name")]
        public string DbColName { get; set; }
        [Column("template_column_name")]
        [JsonProperty("template_column_name")]
        public string TemplateColName { get; set; }
        [Column("is_mandatory")]
        [JsonProperty("is_mandatory")]
        public bool IsMandatory { get; set; }
        [Column("db_column_data_type")]
        [JsonProperty("db_column_data_type")]
        public string DBColumnDataType { get; set; }
        [Column("max_length")]
        [JsonProperty("max_length")]
        public string CharacterMaxLength { get; set; }
        [Column("is_dropdown")]
        [JsonProperty("is_dropdown")]
        public bool IsDropDown { get; set; }
        [Column("column_sequence")]
        [JsonProperty("column_sequence")]
        public int ColumnOrder { get; set; }
        [Column("is_nullable")]
        [JsonProperty("is_nullable")]
        public bool IsNullable { get; set; }

        [NotMapped]
        [Column("is_template_column_required")]
        [JsonProperty("is_template_column_required")]
        public bool is_template_column_required { get; set; }

        [Column("default_value")]
        [JsonProperty("default_value")]
        public string DefaultValue { get; set; }

        [Column("min_value")]
        [JsonProperty("min_value")]
        public string min_value { get; set; }

        [Column("max_value")]
        [JsonProperty("max_value")]
        public string max_value { get; set; }

    }
}