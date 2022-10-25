using AlicjowyBackendv3.Models;

namespace AlicjowyBackendv3.Helpers
{
    public class ReceiptsReadDataTransferObject : ReceiptsModel
    {
        //public string categoryName { get; set; }
        public virtual CategoriesModel category { get; set; }
    }
}
