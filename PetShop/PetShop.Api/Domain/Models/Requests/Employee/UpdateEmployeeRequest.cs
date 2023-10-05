namespace PetShop.Api.Domain.Models.Requests.Employee
{
    public class UpdateEmployeeRequest : CreateEmployeeRequest
    {
        public int Id { get; set; }
    }
}
