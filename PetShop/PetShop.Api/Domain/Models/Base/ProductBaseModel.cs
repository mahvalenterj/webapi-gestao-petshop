
namespace PetShop.Api.Domain.Models.Base
{
    public class ProductBaseModel
    {
        public string ProductName { get; set; }

        public string ProductPrice { get; set; }

        public string ProductQuantity { get; set; }

        public int ProductId { get; set; }
      
        public DateTime ProductBestBefore { get; set; }
        public bool ProductIsDeleted { get; set; }
    }
}
