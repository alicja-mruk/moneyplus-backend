using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlicjowyBackendv3.Models
{
    public partial class Expense
    {
        public virtual Category Category { get; set; } = null!;
        public string id { get; set; } = null!;
        [JsonIgnore]
        public string userGuid { get; set; } = null!;
        [JsonIgnore]
        public long categoryId { get; set; }
        public string name { get; set; } = null!;
        public decimal value { get; set; }
        public DateTime creationDate { get; set; }

        [JsonIgnore]
        public virtual User UserGu { get; set; } = null!;
    }
}
