using FluentAssertions;
using PetShop.Api.Domain.Models.Responses.Employee;
using PetShop.Api.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace PetShop.Api.Tests
{
    public class EmployeeControllerTests : IntegrationTestBase
    {
        private const string Route = "/api/employees";

        private static object NewEmployee(string email = "carlos@petshop.com") => new
        {
            employeeName = "Carlos Lima",
            employeeEmail = email
        };

        private async Task<int> CreateEmployeeAsync(string email = "carlos@petshop.com")
        {
            var response = await Client.PostAsJsonAsync(Route, NewEmployee(email));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            return int.Parse(response.Headers.Location!.Segments.Last());
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var response = await Client.PostAsJsonAsync(Route, NewEmployee());

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
            var body = await response.Content.ReadFromJsonAsync<CreateEmployeeResponse>();
            body!.EmployeeName.Should().Be("Carlos Lima");
            body.EmployeeEmail.Should().Be("carlos@petshop.com");
        }

        [Fact]
        public async Task Create_WithDuplicatedEmail_ReturnsUnprocessableEntity()
        {
            await CreateEmployeeAsync();

            var response = await Client.PostAsJsonAsync(Route, NewEmployee());

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            var problem = await response.Content.ReadAsStringAsync();
            problem.Should().Contain("E-mail já cadastrado");
        }

        [Fact]
        public async Task Create_WithInvalidEmail_ReturnsUnprocessableEntity()
        {
            var response = await Client.PostAsJsonAsync(Route, NewEmployee("email-invalido"));

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            var problem = await response.Content.ReadAsStringAsync();
            problem.Should().Contain("É obrigatório um e-mail válido");
        }

        [Fact]
        public async Task Get_ReturnsCreatedEmployees()
        {
            await CreateEmployeeAsync("carlos@petshop.com");
            await CreateEmployeeAsync("bia@petshop.com");

            var response = await Client.GetAsync(Route);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetEmployeeResponse>();
            body!.Employees.Select(e => e.EmployeeEmail)
                .Should().BeEquivalentTo("carlos@petshop.com", "bia@petshop.com");
        }

        [Fact]
        public async Task GetById_WhenExists_ReturnsOk()
        {
            var id = await CreateEmployeeAsync();

            var response = await Client.GetAsync($"{Route}/{id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetEmployeeByIdResponse>();
            body!.EmployeeEmail.Should().Be("carlos@petshop.com");
        }

        [Fact]
        public async Task GetById_WhenMissing_ReturnsNotFound()
        {
            var response = await Client.GetAsync($"{Route}/999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WhenExists_ReturnsOk()
        {
            var id = await CreateEmployeeAsync();

            var response = await Client.PutAsJsonAsync(Route, new
            {
                id,
                employeeName = "Carlos Souza",
                employeeEmail = "carlos.souza@petshop.com"
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<UpdateEmployeeResponse>();
            body!.EmployeeName.Should().Be("Carlos Souza");

            var stored = await Client.GetFromJsonAsync<GetEmployeeByIdResponse>($"{Route}/{id}");
            stored!.EmployeeEmail.Should().Be("carlos.souza@petshop.com");
        }

        [Fact]
        public async Task Update_WhenMissing_ReturnsNotFound()
        {
            var response = await Client.PutAsJsonAsync(Route, new
            {
                id = 999,
                employeeName = "Carlos Souza",
                employeeEmail = "carlos.souza@petshop.com"
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenExists_ReturnsNoContent()
        {
            var id = await CreateEmployeeAsync();

            var response = await Client.DeleteAsync($"{Route}/{id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            (await Client.GetAsync($"{Route}/{id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenMissing_ReturnsNotFound()
        {
            var response = await Client.DeleteAsync($"{Route}/999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
