using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlicjowyBackendv3.Models
{
    public partial class Category
    {
        public Category()
        {
            Expenses = new HashSet<Expense>();
            Receipts = new HashSet<Receipt>();
            UserCategories = new HashSet<UserCategory>();
        }

        public long id { get; set; }
        public string categoryName { get; set; } = null!;
        public string iconName { get; set; } = null!;
        public string color { get; set; } = null!;
        public string typeOfCategory { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Expense> Expenses { get; set; }
        [JsonIgnore]
        public virtual ICollection<Receipt> Receipts { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserCategory> UserCategories { get; set; }
    }
}
