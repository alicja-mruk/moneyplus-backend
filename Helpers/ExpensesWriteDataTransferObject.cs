using AlicjowyBackendv3.Models;

namespace AlicjowyBackendv3.Helpers
{
    public class ExpensesWriteDataTransferObject : Expense
    {
        public int categoryId { get; set; }
    }
}
