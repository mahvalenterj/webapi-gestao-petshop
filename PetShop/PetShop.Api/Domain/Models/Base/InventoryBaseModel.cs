using PetShop.Api.Domain.Entities;

namespace PetShop.Api.Domain.Models.Base
{
    public class InventoryBaseModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
