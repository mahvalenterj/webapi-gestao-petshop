using FluentAssertions;
using PetShop.Api.Domain.Entities;
using PetShop.Api.Domain.Models.Responses;
using PetShop.Api.Domain.Models.Responses.Pet;
using PetShop.Api.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace PetShop.Api.Tests
{
    public class PetsAndInventoryControllerTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetPetById_WhenExists_ReturnsOk()
        {
            var pet = new Pet
            {
                Name = "Thor",
                AnimalType = "Cachorro",
                Breed = "Labrador",
                Client = new Client { Name = "Ana Souza", Email = "ana@petshop.com", Cpf = "12345678901" }
            };
            await Factory.SeedAsync(context =>
            {
                context.Pets.Add(pet);
                return Task.CompletedTask;
            });

            var response = await Client.GetAsync($"/api/pets/{pet.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetPetByIdResponse>();
            body!.PetName.Should().Be("Thor");
            body.PetAnimalType.Should().Be("Cachorro");
            body.PetBreed.Should().Be("Labrador");
        }

        [Fact]
        public async Task GetPetById_WhenMissing_ReturnsNotFound()
        {
            var response = await Client.GetAsync("/api/pets/999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetInventory_ReturnsOkWithItems()
        {
            await Factory.SeedAsync(context =>
            {
                context.Inventory.Add(new Inventory
                {
                    Quantity = 30,
                    Product = new Product
                    {
                        Name = "Areia Higiênica",
                        BestBefore = new DateTime(2028, 1, 1),
                        Price = "39.90",
                        Quantity = "30"
                    }
                });
                return Task.CompletedTask;
            });

            var response = await Client.GetAsync("/api/estoque");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetInventoryResponse>();
            body!.Inventory.Should().ContainSingle()
                .Which.Quantity.Should().Be(30);
        }

        [Fact]
        public async Task Swagger_ExposesDocumentWithAccentedTexts()
        {
            var response = await Client.GetAsync("/swagger/v1/swagger.json");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var document = await response.Content.ReadAsStringAsync();
            document.Should().Contain("Documentação de WebApi Petshop");
            document.Should().Contain("A documentação relata os métodos e utilizações desta API");
        }
    }
}
