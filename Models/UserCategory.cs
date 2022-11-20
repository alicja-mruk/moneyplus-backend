using System;
using System.Collections.Generic;

namespace AlicjowyBackendv3.Models
{
    public partial class UserCategory
    {
        public string id { get; set; } = null!;
        public string userGuid { get; set; } = null!;
        public long categoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual User UserGu { get; set; } = null!;
    }
}
