using FluentAssertions;
using PetShop.Api.Domain.Models.Responses.Product;
using PetShop.Api.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace PetShop.Api.Tests
{
    public class ProductsControllerTests : IntegrationTestBase
    {
        private const string Route = "/api/produtos";

        private static object NewProduct(string name = "Ração Premium") => new
        {
            productName = name,
            productBestBefore = new DateTime(2027, 5, 10),
            productPrice = "149.90",
            productQuantity = "12"
        };

        private async Task<CreateProductResponse> CreateProductAsync(string name = "Ração Premium")
        {
            var response = await Client.PostAsJsonAsync(Route, NewProduct(name));
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            return (await response.Content.ReadFromJsonAsync<CreateProductResponse>())!;
        }

        [Fact]
        public async Task Create_ReturnsCreatedWithLocationAndBody()
        {
            var response = await Client.PostAsJsonAsync(Route, NewProduct());

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var body = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
            body!.ProductId.Should().BePositive();
            body.ProductName.Should().Be("Ração Premium");
            body.ProductBestBefore.Should().Be(new DateTime(2027, 5, 10));
            body.ProductPrice.Should().Be("149.90");
            body.ProductQuantity.Should().Be("12");
            response.Headers.Location.Should().NotBeNull();
            response.Headers.Location!.AbsolutePath.Should().Be($"{Route}/{body.ProductId}");
        }

        [Fact]
        public async Task Create_WithEmptyName_ReturnsUnprocessableEntity()
        {
            var response = await Client.PostAsJsonAsync(Route, NewProduct(string.Empty));

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            var problem = await response.Content.ReadAsStringAsync();
            problem.Should().Contain("O nome do Produto deve conter entre 2 e 300 caracteres");
        }

        [Fact]
        public async Task Get_ReturnsCreatedProductsWithPriceAndQuantity()
        {
            await CreateProductAsync("Ração Premium");
            await CreateProductAsync("Coleira");

            var response = await Client.GetAsync(Route);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetProductsResponse>();
            body!.Products.Should().HaveCount(2);
            body.Products.Select(p => p.ProductName).Should().BeEquivalentTo("Ração Premium", "Coleira");
            body.Products.Should().OnlyContain(p => p.ProductPrice == "149.90" && p.ProductQuantity == "12");
        }

        [Fact]
        public async Task GetById_WhenExists_ReturnsOk()
        {
            var created = await CreateProductAsync();

            var response = await Client.GetAsync($"{Route}/{created.ProductId}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<GetProductByIdResponse>();
            body!.ProductId.Should().Be(created.ProductId);
            body.ProductName.Should().Be(created.ProductName);
            body.ProductIsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task GetById_WhenMissing_ReturnsNotFound()
        {
            var response = await Client.GetAsync($"{Route}/999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenExists_ReturnsNoContent()
        {
            var created = await CreateProductAsync();

            var response = await Client.DeleteAsync($"{Route}/{created.ProductId}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_WhenMissing_ReturnsNotFound()
        {
            var response = await Client.DeleteAsync($"{Route}/999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_HidesProductFromListAndGetById()
        {
            var deleted = await CreateProductAsync("Shampoo");
            var kept = await CreateProductAsync("Petisco");

            await Client.DeleteAsync($"{Route}/{deleted.ProductId}");

            var list = await Client.GetFromJsonAsync<GetProductsResponse>(Route);
            list!.Products.Select(p => p.ProductId).Should().Equal(kept.ProductId);

            var byId = await Client.GetAsync($"{Route}/{deleted.ProductId}");
            byId.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var deleteAgain = await Client.DeleteAsync($"{Route}/{deleted.ProductId}");
            deleteAgain.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
