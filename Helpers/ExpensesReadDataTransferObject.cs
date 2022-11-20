using AlicjowyBackendv3.Models;

namespace AlicjowyBackendv3.Helpers
{
    public class ExpensesReadDataTransferObject : Expense
    {
        public CategoryExtension Category { get; set; }
    }
}
