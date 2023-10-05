namespace PetShop.Api.Domain.Entities
{
    public class Pet
    {
        public int Id { get; set; }

        public string AnimalType { get; set; }

        public string Name { get; set; }

        public string? Breed { get; set; }

        public int ClientId { get; set; }

        public Client Client { get; set; }
    }
}
