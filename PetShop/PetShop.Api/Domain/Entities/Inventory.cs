namespace PetShop.Api.Domain.Entities
{
    public class Inventory
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
