namespace PetShop.Api.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BestBefore { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public bool IsDeleted { get; set; }
    }
}
