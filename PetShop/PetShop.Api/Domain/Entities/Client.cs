namespace PetShop.Api.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Cpf { get; set; }

        public List<Pet> Pets { get; set; }
    }
}
