using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AlicjowyBackendv3.Models
{
    public partial class User
    {
        public User()
        {
            Expenses = new HashSet<Expense>();
            Receipts = new HashSet<Receipt>();
            UserCategories = new HashSet<UserCategory>();
        }

        public string id { get; set; } = null!;
        public string firstName { get; set; } = null!;
        public string lastName { get; set; } = null!;
        [JsonIgnore]
        public string passwordHash { get; set; } = null!;
        [JsonIgnore]
        public string passwordSalt { get; set; } = null!;
        public short age { get; set; }
        public string email { get; set; } = null!;
        [JsonIgnore]
        public string? refreshToken { get; set; }
        [JsonIgnore]
        public DateTime? tokenExpires { get; set; }

        [JsonIgnore]
        public virtual ICollection<Expense> Expenses { get; set; }
        [JsonIgnore]
        public virtual ICollection<Receipt> Receipts { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserCategory> UserCategories { get; set; }
    }
}
