using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace AlicjowyBackendv3.Models
{
    public class ExpensesModel
    {
        [Key]
        public string id { get; set; }
        //[JsonIgnore]
        //public string userGuid { get; set; }
        //[JsonIgnore]
        //public int categoryId { get; set; }
        public string expenseName { get; set; }
        public string expenseValue { get; set; }
        public DateTime creationDate { get; set; }
    }
}
