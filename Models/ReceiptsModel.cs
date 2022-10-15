using System.ComponentModel.DataAnnotations;

namespace AlicjowyBackendv3.Models
{
    public class ReceiptsModel
    {
        [Key]
        public string receiptsGuid { get; set; }
        //public string userGuid { get; set; }
        //public int categoryId { get; set; }
        public string receiptsName { get; set; }
        public string receiptsValue { get; set; }
        public DateTime creationDate { get; set; }
    }
}
