using PetShop.Api.Domain.Models.Base;

namespace PetShop.Api.Domain.Models.Responses.Client
{
    public class GetClientResponse
    {
        public IEnumerable<ClientBaseModel> Clients { get; set; }
    }
}
