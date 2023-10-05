using PetShop.Api.Domain.Models.Requests.Client;

namespace PetShop.Api.Domain.Models.Requests.Client
{
    public class UpdateClientRequest : CreateClientRequest
    {
        public int Id { get; set; }
    }    
}
