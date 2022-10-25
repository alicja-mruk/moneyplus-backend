using System.ComponentModel.DataAnnotations;

namespace AlicjowyBackendv3.Models
{
    public class CategoriesModel
    {
        [Key]
        public string id { get; set; }
        public string categoryName { get; set; }
        public string iconName { get; set; }
        public string color { get; set; }
        public string typeOfCategory { get; set; }
    }
}
