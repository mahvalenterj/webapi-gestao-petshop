using PetShop.Api.Domain.Entities;

namespace PetShop.Api.Domain.Models.Base
{
    public class ClientBaseModel
    {
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientCpf { get; set; }
        public List<Pet> ClientPets { get; set; }
    }
}
