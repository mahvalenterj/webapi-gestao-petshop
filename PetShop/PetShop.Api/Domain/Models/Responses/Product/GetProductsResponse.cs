using PetShop.Api.Domain.Models.Base;

namespace PetShop.Api.Domain.Models.Responses.Product
{
    public class GetProductsResponse
    {
        public IEnumerable<ProductBaseModel> Products { get; set; }
    }
}
