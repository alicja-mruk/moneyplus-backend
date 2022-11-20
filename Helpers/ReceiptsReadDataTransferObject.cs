using AlicjowyBackendv3.Models;

namespace AlicjowyBackendv3.Helpers
{
    public class ReceiptsReadDataTransferObject : Receipt
    {
        public CategoryExtension Category { get; set; }
    }
}
