using PetShop.Api.Domain.Models.Base;

namespace PetShop.Api.Domain.Models.Responses
{
        public class GetInventoryResponse
        {
            public IEnumerable<InventoryBaseModel> Inventory { get; set; }
        }
}
