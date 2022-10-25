using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AlicjowyBackendv3.Models
{
    public class ReceiptsModel
    {
        [Key]
        public string id { get; set; }
        //public string userGuid { get; set; }
        //public int categoryId { get; set; }
        public string receiptsName { get; set; }
        public string receiptsValue { get; set; }
        public DateTime creationDate { get; set; }
    }
}
