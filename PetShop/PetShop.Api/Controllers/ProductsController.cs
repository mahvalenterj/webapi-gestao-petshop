using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using PetShop.Api.Database;
using PetShop.Api.Domain.Entities;
using PetShop.Api.Domain.Models.Base;
using PetShop.Api.Domain.Models.Requests.Product;
using PetShop.Api.Domain.Models.Responses.Product;
using PetShop.Api.Domain.Validators;
using System.Net.Mime;

namespace PetShop.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    [Consumes(MediaTypeNames.Application.Json)]
    public class ProductsController : ControllerBase
    {
        private readonly IPetShopDbContext petShopDbContext;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IPetShopDbContext petShopDbContext, ILogger<ProductsController> logger)
        {
            this.petShopDbContext = petShopDbContext;
            this.logger = logger;
        }

        /// <summary>
        /// Cria um novo produto com base nos dados fornecidos.
        /// </summary>
        /// <param name="request">Dados do produto a serem criados.</param>
        /// <response code="201">Produto criado com sucesso.</response>
        /// <response code="422">Dados do produto inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult CreateProduct([FromBody] CreateProductRequest request)
        {
            logger.LogTrace("Iniciou o método Post-Products");

            var entity = new Product
            {
                Name = request.ProductName,
                BestBefore = request.ProductBestBefore,
                Price = request.ProductPrice,
                Quantity = request.ProductQuantity,
            };

            var validator = new ProductValidator(petShopDbContext);
            var validationResult = validator.Validate(entity);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(statusCode: StatusCodes.Status422UnprocessableEntity,
                    modelStateDictionary: ModelState);
            }

            petShopDbContext.Products.Add(entity);
            petShopDbContext.SaveChanges();

            var responseModel = new CreateProductResponse
            {
                ProductId = entity.Id,
                ProductName = entity.Name,
                ProductBestBefore = entity.BestBefore,
                ProductPrice = entity.Price,
                ProductQuantity = entity.Quantity
            };

            logger.LogTrace("Finalizou o método Post-Products");
            return CreatedAtAction(nameof(GetProductById), new { id = entity.Id }, responseModel);
        }

        /// <summary>
        /// Marca o produto com o ID informado como deletado.
        /// </summary>
        /// <param name="id">Id do Produto</param>
        /// <response code="204">Produto marcado como deletado.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public IActionResult DeleteById(int id)
        {
            var entity = petShopDbContext.Products.Find(id);

            if (entity == null)
                return NotFound();

            entity.IsDeleted = true;
            petShopDbContext.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Retorna todos os produtos cadastrados.
        /// </summary>
        /// <response code="200">Lista com todos os produtos cadastrados. Pode ser uma lista
        /// vazia caso nao haja produto cadastrado.</response>
        [HttpGet]
        [ProducesResponseType(typeof(GetProductsResponse), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult GetProducts()
        {
            logger.LogTrace("Iniciou o método Get Products.");

            var products = new GetProductsResponse();

            products.Products = petShopDbContext.Products
               .Select(x => new ProductBaseModel
               {
                   ProductId = x.Id,
                   ProductName = x.Name,
                   ProductBestBefore = x.BestBefore,
                   ProductPrice = x.Price,
                   ProductQuantity = x.Quantity
               })
               .ToList();

            logger.LogTrace("Finalizou o método Get Products.");
            return Ok(products);
        }

        /// <summary>
        /// Retorna as informações do produto com o ID.
        /// </summary>
        /// <param name="id">Id do Produto</param>
        /// <response code="200">Retorna os dados do produto, quando encontrado.</response>
        /// <response code="404">Produto não encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GetProductByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult GetProductById(int id)
        {
            logger.LogTrace("Iniciou o método get");

            var entity = petShopDbContext.Products.Find(id);

            if (entity == null)
                return NotFound();

            var model = new GetProductByIdResponse
            {
                ProductName = entity.Name,
                ProductId = entity.Id,
                ProductBestBefore = entity.BestBefore,
                ProductPrice = entity.Price,
                ProductIsDeleted = entity.IsDeleted,
                ProductQuantity = entity.Quantity
            };

            logger.LogTrace("Finalizou o método get");
            return Ok(model);
        }
    }
}
