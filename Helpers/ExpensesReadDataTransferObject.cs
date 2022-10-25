using AlicjowyBackendv3.Models;

namespace AlicjowyBackendv3.Helpers
{
    public class ExpensesReadDataTransferObject : ExpensesModel
    {
        //public string categoryName { get; set; }
        public virtual CategoriesModel category { get; set; }
    }
}
