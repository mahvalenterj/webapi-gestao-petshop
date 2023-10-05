using PetShop.Api.Domain.Models.Base;

namespace PetShop.Api.Domain.Models.Responses.Employee
{
    public class GetEmployeeResponse
    {
        public IEnumerable<EmployeeBaseModel> Employees { get; set; }
    }
}
