using FluentAssertions;
using PetShop.Api.Domain.Models.Responses.Client;
using PetShop.Api.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PetShop.Api.Tests
{
    public class ClientsControllerTests : IntegrationTestBase
    {
        private const string Route = "/api/cliente";

        private static object NewClient(string cpf = "12345678901") => new
        {
            clientName = "Ana Souza",
            clientEmail = "ana@petshop.com",
            clientCpf = cpf
        };

        private async Task<int> CreateClientAsync()
        {
            var response = await Client.PostAsJsonAsync(Route, NewClient());
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            return int.Parse(response.Headers.Location!.Segments.Last());
        }

        [Fact]
        public async Task Create_ReturnsCreatedWithDto()
        {
            var response = await Client.PostAsJsonAsync(Route, NewClient());

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var properties = json.RootElement.EnumerateObject().Select(p => p.Name);
            properties.Should().BeEquivalentTo("clientName", "clientEmail", "clientCpf");
            json.RootElement.GetProperty("clientName").GetString().Should().Be("Ana Souza");
            json.RootElement.GetProperty("clientEmail").GetString().Should().Be("ana@petshop.com");
            json.RootElement.GetProperty("clientCpf").GetString().Should().Be("12345678901");
        }

        [Fact]
        public async Task Create_WithInvalidCpf_ReturnsValidationError()
        {
            var response = await Client.PostAsJsonAsync(Route, NewClient(cpf: "123"));

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            var problem = await response.Content.ReadAsStringAsync();
            problem.Should().Contain("ClientCpf");
        }

        [Fact]
        public async Task Get_ReturnsCreatedClients()
        {
            await CreateClientAsync();

            var response = await Client.GetAsync(Route);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetClientResponse>();
            body!.Clients.Should().ContainSingle()
                .Which.ClientEmail.Should().Be("ana@petshop.com");
        }

        [Fact]
        public async Task GetById_WhenExists_ReturnsOk()
        {
            var id = await CreateClientAsync();

            var response = await Client.GetAsync($"{Route}/{id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetClientByIdResponse>();
            body!.ClientName.Should().Be("Ana Souza");
            body.ClientCpf.Should().Be("12345678901");
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
            var id = await CreateClientAsync();

            var response = await Client.PutAsJsonAsync(Route, new
            {
                id,
                clientName = "Ana Lima",
                clientEmail = "ana.lima@petshop.com",
                clientCpf = "12345678901"
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<UpdateClientResponse>();
            body!.ClientName.Should().Be("Ana Lima");
            body.ClientEmail.Should().Be("ana.lima@petshop.com");

            var stored = await Client.GetFromJsonAsync<GetClientByIdResponse>($"{Route}/{id}");
            stored!.ClientName.Should().Be("Ana Lima");
        }

        [Fact]
        public async Task Update_WhenMissing_ReturnsNotFound()
        {
            var response = await Client.PutAsJsonAsync(Route, new
            {
                id = 999,
                clientName = "Ana Lima",
                clientEmail = "ana.lima@petshop.com",
                clientCpf = "12345678901"
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenExists_ReturnsNoContent()
        {
            var id = await CreateClientAsync();

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
